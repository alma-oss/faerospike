namespace Lmc.Aerospike

[<RequireQualifiedAccess>]
module Store =
    open Aerospike.Client

    let connect (configuration: ConnectionConfiguration) =
        new AerospikeClient(configuration.Host, configuration.Port)

    let tryToConnect logAerospike configuration =
        try
            connect configuration |> Some
        with
        | :? AerospikeException as e ->
            logAerospike <| sprintf "%A" e
            None

    let rec tryToConnectWithReconnects logAerospike attempts configuration =
        let attemptLeft = (attempts - 1)
        logAerospike <| sprintf "Try to connect - attempt left: %i" attemptLeft
        if attemptLeft <= 0 then
            failwithf "Connection to aerospike could not be estabilished"

        match configuration |> tryToConnect logAerospike with
        | Some aerospike -> aerospike
        | None ->
            logAerospike "Connection failed, waiting for 10s ..."
            System.Threading.Thread.Sleep (10 * 1000)
            tryToConnectWithReconnects logAerospike attemptLeft configuration

    let connectWithReconnects logAerospike configuration =
        configuration |> tryToConnectWithReconnects logAerospike 10

    let private createKey configuration (keyValue: string) =
        Key(configuration.Namespace, configuration.SetName, keyValue)

    let put (client: AerospikeClient) key value =
        let policy = WritePolicy()
        policy.recordExistsAction <- RecordExistsAction.UPDATE
        policy.expiration <- -1 // never expires - see https://www.aerospike.com/apidocs/csharp/html/F_Aerospike_Client_WritePolicy_expiration.htm

        client.Put(policy, key, value)

    let putAs<'Value> client configuration key name (value: 'Value) =
        let key = key |> createKey configuration
        let value = [| Bin(name, value) |]

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
        let policy = new ScanPolicy()
        policy.includeBinData <- true

        client.ScanAll(policy, configuration.Namespace, configuration.SetName, ScanCallback(f))
