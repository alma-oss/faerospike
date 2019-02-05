F-Aerospike
=======

Library for Aerospike.

## Install
```
dotnet add package -s $NUGET_SERVER_PATH Lmc.Aerospike
```
Where `$NUGET_SERVER_PATH` is the URL of nuget server
- it should be http://development-nugetserver-common-stable.service.devel1-services.consul:31794 (_make sure you have a correct port, since it changes with deployment_)
- see http://consul-1.infra.pprod/ui/devel1-services/services/development-nugetServer-common-stable for detailed information (and port)

## Use
```fs
open Aerospike
open Aerospike.Store

let connectionConfiguration = {
    Host = "127.0.0.1"
    Port = 3000
}

let configuration = {
    Namespace = "aerospike_namespace"
    SetName = "aerospike_set_name"
}

use client = connect connectionConfiguration            // connection is disposable, so if it is not used anymore, it will be disposed (disconnected)

let log = printfn "%s"                                  // simple stdout log
let storeState = storeState log client configuration    // storeState function with baked in dependencies

storeState true "some_key"                              // store value `true` for key `some_key` to the aerospike
```

## Release
1. Increment version in `src/Aerospike.fsproj`
2. Update `CHANGELOG.md`
3. Commit new version and tag it
4. Run `$ fake build target release`

## Development
### Requirements
- [dotnet core](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)
- [FAKE](https://fake.build/fake-gettingstarted.html)

### Build
```bash
fake build
```

### Watch
```bash
fake build target watch
```
