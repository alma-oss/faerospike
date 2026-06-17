# Preferred Patterns

## Core Principles

- Treat the connection as a resource: `connect` hands back a disposable `AerospikeClient`, so own its lifetime with a `use` binding (or dispose it explicitly) rather than letting it leak.
- Build configuration once and thread it through the read/write helpers; `Configuration` (namespace + set) and the connection settings are plain records, cheap to pass around.
- Prefer the typed helpers (`putAs`, `findValueOf`) over hand-rolling `Bin`/`Record` access; they centralise the bin name and the runtime cast.
- Let `parse` turn untrusted strings (env vars, config files) into typed values at the boundary, and handle the `None` case there instead of deeper in the code.

## Recommended API Usage

- Connecting: call `Store.connect loggerFactory configuration`. It installs the logging callback and returns `Result<AerospikeClient, ConnectionError>`; match on the result before using the client. See `examples.md` → Basic Connect and Write.
- Resilient connecting: in long-running hosts that may start before Aerospike, use `Store.connectWithReconnects loggerFactory availableAttempts configuration`, which retries inside `asyncResult`. See `examples.md` → Connecting With Reconnects.
- Writing: `putAs client configuration key binName value` is the typed convenience over the lower-level `put client key bin`. The write policy is fixed to update-existing, never-expire, random replica, commit-all. See `examples.md` → Basic Connect and Write.
- Reading a typed value: `findValueOf<'T> client configuration binName key` returns `'T option`, yielding `None` when the record or bin is missing or the cast fails. For a string-rendered value regardless of stored type, use `findValueAsString`. See `examples.md` → Reading Values Back.
- Raw record access: `findData client configuration key` returns `Record option` when you need more than one bin or the whole record.
- Scanning: `iter client configuration callback` runs a full scan, invoking `Key -> Record -> unit` per record with bin data included. See `examples.md` → Scanning All Records.

## Error Handling

- `connect` converts an `AerospikeException` into `Error (ConnectionError e)`; render any `ConnectionError` for logs or messages with `ConnectionError.format`.
- `connectWithReconnects` surfaces the final failure through `asyncResult` once attempts are exhausted, so bind it with `let!`/`do!` inside an `asyncResult` block and handle the terminal `Error`.
- Reads return `option`, not `Result`; a missing key, missing bin, or failed cast all collapse to `None`, so decide at the call site how absence differs from a value.

## Composition

- The helpers are designed for partial application: apply `client` and `configuration` once to produce task-specific functions (e.g. a pre-bound writer or reader) and pass those around. See `examples.md` → Composing Bound Helpers.
- Keep the bin name as an explicit argument so a single configured store can hold several named values per key.

## Integration with Other Libraries

- Pass an `ILoggerFactory` from `Microsoft.Extensions.Logging`; it drives both the connection-level logger and the Aerospike client's internal log callback (levels are mapped automatically).
- `connectWithReconnects` relies on `Feather.ErrorHandling` (`asyncResult`, `AsyncResult.sleep`, `AsyncResult.ofError`); compose it within other `asyncResult` workflows.
- `Key`, `Bin`, `Record`, and `AerospikeClient` come from `Aerospike.Client`; drop down to that client directly for features this wrapper does not cover.

## Naming Conventions

- Modules use `[<RequireQualifiedAccess>]`, so always qualify calls (`Store.put`, `Configuration.value`, `NodeConnection.parse`).
- Rendering a typed config to its string form is consistently named `value` (`Configuration.value`, `ConnectionConfiguration.value`, `NodeConnection.value`).
- Generic typed helpers carry an explicit type argument suffix (`putAs<'Value>`, `findValueOf<'Value>`).

## Testing Recommendations

- The repository ships no test project; when adding tests in a consumer, cover the pure functions first — `NodeConnection.parse`, `ConnectionConfiguration.parse`, and the `value` renderers are deterministic and need no running cluster.
- For the `Store` helpers, integration-test against a real or containerised Aerospike instance rather than mocking the sealed `AerospikeClient`; assert round-trips with `putAs` followed by `findValueOf`.
