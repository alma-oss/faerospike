namespace Lmc.Aerospike

type ConnectionConfiguration = {
    Host: string
    Port: int
}

[<RequireQualifiedAccess>]
module ConnectionConfiguration =
    let toString configuration =
        sprintf "%s:%i" configuration.Host configuration.Port

type Configuration = {
    Namespace: string
    SetName: string
}

[<RequireQualifiedAccess>]
module Configuration =
    let toString configuration =
        sprintf "%s/%s" configuration.Namespace configuration.SetName
