---
name: faerospike
description: Use whenever generating or reviewing F# code that talks to an Aerospike key-value store via the Alma.Aerospike library — connecting (Store.connect, Store.connectWithReconnects), writing (Store.put, Store.putAs), reading (Store.findData, Store.findValueOf, Store.findValueAsString), or scanning (Store.iter). Trigger also on mentions of AerospikeClient, ConnectionConfiguration, NodeConnection, SingleNode/Cluster, Configuration (Namespace/SetName), Bin, ConnectionError, or ILoggerFactory-based Aerospike setup, and on parsing connection strings like "127.0.0.1:3000".
---

# F-Aerospike

Library: [alma-oss/faerospike](https://github.com/alma-oss/faerospike)
NuGet: `Alma.Aerospike`

## Purpose

`Alma.Aerospike` is an F# functional wrapper around the official Aerospike .NET client. It exposes typed connection configuration, a disposable client created from single-node or cluster settings, and small helper functions for storing, reading, and scanning key-value records in an Aerospike namespace/set.

## When to Use

- Connecting to an Aerospike single node or cluster from F#.
- Writing typed values to records and reading them back by key.
- Scanning all records in a namespace/set.
- Parsing connection strings or namespace/set settings into typed configuration.

## When NOT to Use

- You need Aerospike features not wrapped here (secondary indexes, batch ops, UDFs, expressions, transactions) — use `Aerospike.Client` directly.
- You are not using Aerospike at all.
- You want to follow the README's `connect`/`storeState` snippet — it is outdated; rely on the actual API below.

## Main Concepts

- `NodeConnection` — a single `{ Host; Port }`; `parse` reads `"host:port"`, `value` renders it back.
- `ConnectionConfiguration` — `SingleNode of NodeConnection` or `Cluster of NodeConnection list`; `parse` reads a comma-separated connection string, `value` renders it.
- `Configuration` — `{ Namespace; SetName }` identifying where records live; `value` renders `"namespace/setName"`.
- `AerospikeClient` — the underlying client returned by `connect`; it is `IDisposable`, bind it with `use`.
- `ConnectionError` — `ConnectionError of AerospikeException` or `ConnectionNotEstabilished`; `format` produces a readable string.
- `Store.connect` — wraps client creation, returning `Result<AerospikeClient, ConnectionError>` and installing logging from an `ILoggerFactory`.
- `Store.connectWithReconnects` — `asyncResult` retry loop with a fixed 10s wait, decrementing an attempt counter until it succeeds or runs out.
- `Bin` — a named value cell within a record; `putAs` builds one for you from a name and value.
- `Store.iter` — full scan invoking a `Key -> Record -> unit` callback over the configured namespace/set.

## Related Libraries

- `Aerospike.Client` — the underlying official client; types like `AerospikeClient`, `Key`, `Bin`, `Record` come from it.
- `Feather.ErrorHandling` — supplies the `asyncResult` computation expression and `AsyncResult` helpers used by `connectWithReconnects`.
- `Microsoft.Extensions.Logging.Abstractions` — `ILoggerFactory` drives both connection logging and the Aerospike client's internal log callback.

## Keywords for Search

Alma.Aerospike, faerospike, Aerospike, AerospikeClient, key-value store, ConnectionConfiguration, NodeConnection, SingleNode, Cluster, Configuration, Namespace, SetName, Bin, Record, Key, ConnectionError, Store.connect, connectWithReconnects, put, putAs, findData, findValueOf, findValueAsString, iter, ScanAll, ILoggerFactory, asyncResult, connection string parsing

## Reference Files

- For composition principles and recommended API usage, read `references/preferred-patterns.md`.
- For known pitfalls and incorrect assumptions, read `references/anti-patterns.md`.
- For worked code examples, read `references/examples.md`.
