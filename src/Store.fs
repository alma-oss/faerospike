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

    let private put (client: AerospikeClient) key value =
        let policy = WritePolicy()
        policy.recordExistsAction <- RecordExistsAction.UPDATE
        policy.expiration <- -1 // never expires - see https://www.aerospike.com/apidocs/csharp/html/F_Aerospike_Client_WritePolicy_expiration.htm

        client.Put(policy, key, value)

    let storeState log (client: AerospikeClient) (configuration: Configuration) (state: bool) (key: string) =
        log (sprintf "Storing state %A" state)
        let key' =
            key
            |> createKey configuration
        let value = [| Bin("has_ack", if state then 1 else 0) |]

        put client key' value
