namespace Alma.Aerospike

type NodeConnection = {
    Host: string
    Port: int
}

[<RequireQualifiedAccess>]
module NodeConnection =
    let parse = function
        | null | "" -> None
        | node ->
            match node.Split ":" with
            | [| host; port |] ->
                Some {
                    Host = host.Trim ' '
                    Port = int port
                }
            | _ -> None

    let value configuration =
        sprintf "%s:%i" configuration.Host configuration.Port

type ConnectionConfiguration =
    | SingleNode of NodeConnection
    | Cluster of NodeConnection list

[<RequireQualifiedAccess>]
module ConnectionConfiguration =
    /// It parses connection configuration out of a string
    /// Example:
    /// Single Node: "127.0.0.1:3000"
    /// Cluster: "127.0.0.1:3000,127.0.0.2:3000"
    let parse = function
        | null | "" -> None
        | connection ->
            match connection.Split "," with
            | [||] -> None
            | [| single |] -> single |> NodeConnection.parse |> Option.map SingleNode
            | nodes ->
                nodes
                |> List.ofSeq
                |> List.choose NodeConnection.parse
                |> Cluster
                |> Some

    let value = function
        | SingleNode node -> node |> NodeConnection.value
        | Cluster nodes -> nodes |> List.map NodeConnection.value |> String.concat ","

type Configuration = {
    Namespace: string
    SetName: string
}

[<RequireQualifiedAccess>]
module Configuration =
    let value configuration =
        sprintf "%s/%s" configuration.Namespace configuration.SetName
