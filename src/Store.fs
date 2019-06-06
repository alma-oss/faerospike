namespace Aerospike

module Store =
    open Aerospike.Client

    let connect (configuration: ConnectionConfiguration) =
        new AerospikeClient(configuration.Host, configuration.Port)

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
