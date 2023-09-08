F-Aerospike
===========

Library for Aerospike.

## Install

Add following into `paket.dependencies`
```
source https://nuget.pkg.github.com/almacareer/index.json username: "%PRIVATE_FEED_USER%" password: "%PRIVATE_FEED_PASS%"
# LMC Nuget dependencies:
nuget Alma.Aerospike
```

NOTE: For local development, you have to create ENV variables with your github personal access token.
```sh
export PRIVATE_FEED_USER='{GITHUB USERNANME}'
export PRIVATE_FEED_PASS='{TOKEN}'	# with permissions: read:packages
```

Add following into `paket.references`
```
Alma.Aerospike
```

## Use
```fs
open Alma.Aerospike
open Alma.Aerospike.Store

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
1. Increment version in `ServiceIdentification.fsproj`
2. Update `CHANGELOG.md`
3. Commit new version and tag it

## Development
### Requirements
- [dotnet core](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)

### Build
```bash
./build.sh build
```

### Tests
```bash
./build.sh -t tests
```
