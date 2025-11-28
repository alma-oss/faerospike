F-Aerospike
===========

[![NuGet](https://img.shields.io/nuget/v/Alma.Aerospike.svg)](https://www.nuget.org/packages/Alma.Aerospike)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Alma.Aerospike.svg)](https://www.nuget.org/packages/Alma.Aerospike)
[![Tests](https://github.com/alma-oss/faerospike/actions/workflows/tests.yaml/badge.svg)](https://github.com/alma-oss/faerospike/actions/workflows/tests.yaml)

Library for Aerospike.

## Install

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
1. Increment version in `Aerospike.fsproj`
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
