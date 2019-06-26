F-Aerospike
=======

Library for Aerospike.

## Install

Add following into `paket.dependencies`
```
git ssh://git@stash.int.lmc.cz:7999/archi/nuget-server.git master Packages: /nuget/
# LMC Nuget dependencies:
nuget Lmc.Aerospike
```

Add following into `paket.references`
```
Lmc.Aerospike
```

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
5. Go to `nuget-server` repo, run `faket build target copyAll` and push new versions

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
