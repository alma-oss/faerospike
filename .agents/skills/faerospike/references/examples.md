# Examples

All example code for this skill lives here. Examples are ordered by increasing complexity and each is self-contained.

## Basic Connect and Write

```fsharp
open Alma.Aerospike
open Microsoft.Extensions.Logging

let loggerFactory = LoggerFactory.Create(fun b -> b.AddConsole() |> ignore)

let connection = SingleNode { Host = "127.0.0.1"; Port = 3000 }
let configuration = { Namespace = "demo_namespace"; SetName = "demo_set" }

match Store.connect loggerFactory connection with
| Ok client ->
    use client = client
    Store.putAs client configuration "item-1" "enabled" true
| Error e ->
    eprintfn "%s" (ConnectionError.format e)
```

## Parsing Connection Strings

```fsharp
open Alma.Aerospike

// Single node -> SingleNode, cluster (comma-separated) -> Cluster
let singleNode = ConnectionConfiguration.parse "127.0.0.1:3000"
let cluster = ConnectionConfiguration.parse "127.0.0.1:3000,127.0.0.2:3000"

match ConnectionConfiguration.parse "127.0.0.1:3000" with
| Some config -> printfn "parsed: %s" (ConnectionConfiguration.value config)
| None -> eprintfn "invalid connection string"
```

## Reading Values Back

```fsharp
open Alma.Aerospike

let readFlag (client: Aerospike.Client.AerospikeClient) configuration key =
    match Store.findValueOf<bool> client configuration "enabled" key with
    | Some value -> printfn "enabled = %b" value
    | None -> printfn "no value stored for %s" key

let readAsText client configuration key =
    Store.findValueAsString client configuration "enabled" key
    |> Option.defaultValue "<missing>"
```

## Scanning All Records

```fsharp
open Alma.Aerospike
open Aerospike.Client

let printAll (client: AerospikeClient) configuration =
    let onRecord (key: Key) (record: Record) =
        printfn "%O -> %O" key.userKey (record.GetValue "enabled")

    Store.iter client configuration onRecord
```

## Composing Bound Helpers

```fsharp
open Alma.Aerospike

// Bake in client + configuration once, then reuse task-specific functions.
let makeStore client configuration =
    let writeFlag key value = Store.putAs client configuration key "enabled" value
    let readFlag key = Store.findValueOf<bool> client configuration "enabled" key
    writeFlag, readFlag

let useStore client configuration =
    let writeFlag, readFlag = makeStore client configuration
    writeFlag "item-1" true
    readFlag "item-1"
```

## Connecting With Reconnects

```fsharp
open Alma.Aerospike
open Microsoft.Extensions.Logging
open Feather.ErrorHandling

let loggerFactory = LoggerFactory.Create(fun b -> b.AddConsole() |> ignore)

let connection = SingleNode { Host = "127.0.0.1"; Port = 3000 }
let configuration = { Namespace = "demo_namespace"; SetName = "demo_set" }

// Retries up to 5 times, waiting 10s between attempts, inside asyncResult.
let startup = asyncResult {
    let! client = Store.connectWithReconnects loggerFactory 5 connection
    use client = client
    Store.putAs client configuration "item-1" "enabled" true
    return Store.findValueOf<bool> client configuration "enabled" "item-1"
}
```

## Full Workflow

```fsharp
open Alma.Aerospike
open Microsoft.Extensions.Logging

let run (rawConnection: string) =
    let loggerFactory = LoggerFactory.Create(fun b -> b.AddConsole() |> ignore)
    let configuration = { Namespace = "demo_namespace"; SetName = "demo_set" }

    match ConnectionConfiguration.parse rawConnection with
    | None -> Error "invalid connection string"
    | Some connection ->
        match Store.connect loggerFactory connection with
        | Error e -> Error (ConnectionError.format e)
        | Ok client ->
            use client = client
            Store.putAs client configuration "item-1" "enabled" true
            Store.findValueOf<bool> client configuration "enabled" "item-1"
            |> Result.Ok
```
