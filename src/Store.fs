namespace Lmc.Aerospike

open Aerospike.Client

type ConnectionError =
    | ConnectionError of AerospikeException
    | ConnectionNotEstabilished

[<RequireQualifiedAccess>]
module ConnectionError =
    let format = function
        | ConnectionError e -> sprintf "%A" e
        | ConnectionNotEstabilished -> "Connection to aerospike could not be estabilished"

[<RequireQualifiedAccess>]
module Store =
    open Microsoft.Extensions.Logging
    open Lmc.ErrorHandling

    let private connectClient = function
        | SingleNode node -> new AerospikeClient(node.Host, node.Port)
        | Cluster nodes ->
            let policy = ClientPolicy()
            let hosts: Host array =
                nodes
                |> List.map (fun node -> Host(node.Host, node.Port))
                |> List.toArray

            new AerospikeClient(policy, hosts)

    [<RequireQualifiedAccess>]
    module internal Logging =
        let private asLogLevelToLogLevel = function
            | Log.Level.DEBUG -> LogLevel.Debug
            | Log.Level.ERROR -> LogLevel.Error
            | Log.Level.WARN -> LogLevel.Warning
            | Log.Level.INFO -> LogLevel.Information
            | _ -> LogLevel.None

        let private logLevelToAsLogLevel = function
            | LogLevel.Debug | LogLevel.Trace -> Some Log.Level.DEBUG
            | LogLevel.Error | LogLevel.Critical -> Some Log.Level.ERROR
            | LogLevel.Warning -> Some Log.Level.WARN
            | LogLevel.Information -> Some Log.Level.INFO
            | _ -> None

        let setUp (loggerFactory: ILoggerFactory) =
            let logger = loggerFactory.CreateLogger("Aerospike")
            Log.SetCallback(fun level message -> logger.Log(level |> asLogLevelToLogLevel, message))

            [
                LogLevel.Trace
                LogLevel.Debug
                LogLevel.Information
                LogLevel.Warning
                LogLevel.Error
                LogLevel.Critical
                LogLevel.None
            ]
            |> List.tryPick logLevelToAsLogLevel
            |> Option.iter Log.SetLevel

    let connect (loggerFactory: ILoggerFactory) configuration =
        loggerFactory |> Logging.setUp

        try connectClient configuration |> Ok
        with
        | :? AerospikeException as e -> Error (ConnectionError e)

    let rec connectWithReconnects (loggerFactory: ILoggerFactory) (availableAttempts: int) configuration = asyncResult {
        let logger = loggerFactory.CreateLogger("Aerospike")
        logger.LogInformation("Try to connect - attempt left: {availableAttempts}", availableAttempts)

        match configuration |> connect loggerFactory with
        | Ok aerospike -> return aerospike
        | Error e when availableAttempts <= 0 -> return! AsyncResult.ofError e
        | Error e ->
            logger.LogWarning("Connection failed with {error}", e)

            let secondsToWait = 10
            logger.LogInformation("Connection failed, waiting {waitSeconds}s for reconnect...", secondsToWait)
            do! AsyncResult.sleep (secondsToWait * 1000)

            return! connectWithReconnects loggerFactory (availableAttempts - 1) configuration
    }

    let private createKey configuration (keyValue: string) =
        Key(configuration.Namespace, configuration.SetName, keyValue)

    let put (client: AerospikeClient) key (value: Bin) =
        let policy = WritePolicy()
        policy.recordExistsAction <- RecordExistsAction.UPDATE
        policy.expiration <- -1 // never expires - see https://www.aerospike.com/apidocs/csharp/html/F_Aerospike_Client_WritePolicy_expiration.htm
        policy.replica <- Replica.RANDOM
        policy.commitLevel <- CommitLevel.COMMIT_ALL

        client.Put(policy, key, value)

    let putAs<'Value> client configuration key name (value: 'Value) =
        let key = key |> createKey configuration
        let value = Bin(name, value)

        put client key value

    let findData (client: AerospikeClient) configuration key =
        match client.Get(null, key |> createKey configuration) with
        | null -> None
        | data -> Some data

    let findValueOf<'Value> client configuration name key =
        key
        |> findData client configuration
        |> Option.bind (fun data ->
            match data.GetValue(name) with
            | :? 'Value as value -> Some value
            | _ -> None
        )

    let findValueAsString client configuration name key =
        key
        |> findData client configuration
        |> Option.map (fun data ->
            data.GetValue(name).ToString()
        )

    let iter (client: AerospikeClient) configuration (f: Key -> Record -> unit) =
        let policy = ScanPolicy()
        policy.includeBinData <- true

        client.ScanAll(policy, configuration.Namespace, configuration.SetName, ScanCallback(f))
