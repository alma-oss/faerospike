# Anti-Patterns

Each entry is **mistake → why → fix**.

## Following the README snippet

- **Mistake:** Calling `connect connectionConfiguration` and a `storeState log client configuration` function as shown in the README.
- **Why:** That API is outdated. `connect` takes an `ILoggerFactory` first, and `storeState` was removed; the README was not updated.
- **Fix:** Use the current API — `Store.connect loggerFactory configuration` plus `Store.putAs` for writes. See `examples.md` → Basic Connect and Write.

## Leaking the client

- **Mistake:** Binding the connected client with `let` and never disposing it.
- **Why:** `AerospikeClient` is `IDisposable` and holds live cluster connections; leaking it exhausts resources over time.
- **Fix:** Bind with `use` (or dispose explicitly when the lifetime is managed elsewhere).

## Ignoring the connect Result

- **Mistake:** Treating `Store.connect` as if it returns a client directly and using the value without matching.
- **Why:** It returns `Result<AerospikeClient, ConnectionError>`; a failed connection is an `Error`, not an exception you can ignore.
- **Fix:** Match on the `Result` (or bind it in an `asyncResult`/`result` block) and format failures with `ConnectionError.format`.

## Passing a connection string to connect

- **Mistake:** Calling `Store.connect loggerFactory "127.0.0.1:3000"`.
- **Why:** `connect` expects a typed `ConnectionConfiguration`, not a raw string.
- **Fix:** Parse first with `ConnectionConfiguration.parse` and handle the `None` case, then pass the typed value. See `examples.md` → Parsing Connection Strings.

## Expecting reads to throw or return Result

- **Mistake:** Wrapping `findValueOf` / `findData` in try/catch or matching them as `Result`.
- **Why:** They return `option`; a missing key, missing bin, or failed cast all yield `None` with no exception or `Error`.
- **Fix:** Match on `Some`/`None` and decide how absence is handled at the call site.

## Assuming writes create-or-fail or expire

- **Mistake:** Expecting `put`/`putAs` to fail when a record exists, or to set a TTL.
- **Why:** The write policy is fixed to `RecordExistsAction.UPDATE`, `expiration = -1` (never expires), random replica, commit-all — writes upsert and persist indefinitely.
- **Fix:** If you need different semantics (create-only, TTL), drop to `Aerospike.Client` and supply your own `WritePolicy`.

## Reaching past the qualified modules

- **Mistake:** Calling `put`, `parse`, or `value` unqualified after `open Alma.Aerospike`.
- **Why:** Modules are `[<RequireQualifiedAccess>]`; the unqualified names are not in scope.
- **Fix:** Qualify every call (`Store.put`, `ConnectionConfiguration.parse`, `Configuration.value`).

## Mocking the client in tests

- **Mistake:** Trying to unit-test `Store` helpers by mocking `AerospikeClient`.
- **Why:** It is a concrete sealed client from `Aerospike.Client`, not an interface; mocking is brittle or impossible.
- **Fix:** Unit-test the pure `parse`/`value` functions and integration-test the helpers against a real/containerised Aerospike.
