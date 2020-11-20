namespace Lmc.Aerospike

type ConnectionConfiguration = {
    Host: string
    Port: int
}

module ConnectionConfiguration =
    let toString configuration =
        sprintf "%s:%i" configuration.Host configuration.Port

type Configuration = {
    Namespace: string
    SetName: string
}

module Configuration =
    let toString configuration =
        sprintf "%s/%s" configuration.Namespace configuration.SetName
