# Changelog

<!-- There is always Unreleased section on the top. Subsections (Add, Changed, Fix, Removed) should be Add as needed. -->
## Unreleased
- [**BC**] Use net 7.0
- Update dependencies

## 5.0.0 - 2022-11-23
- Update dependencies
    - [**BC**] Aerospike.Client ~> 5.3 (which requires Aerospike.Server ^6.0)

## 4.1.0 - 2022-01-04
- Update dependencies

## 4.0.0 - 2021-12-15
- Use net6.0
- [**BC**] Update dependencies
    - Aerospike.Client ~> 4.2 (which requires Aerospike.Server ^5.0)
- [**BC**] Requires Qualified Access for Modules
- Add/Change connect functions
    - `Store.connect`
    - `Store.connectWithReconnects`
- [**BC**] Remove too specific function `stateStore`
- Add more common functions to read/write
    - `Store.put`
    - `Store.putAs`
    - `Store.findData`
    - `Store.findValueOf`
    - `Store.findValueAsString`
    - `Store.iter`
- [**BC**] Change type `ConnectionConfiguration`
- Add type `NodeConnection`
- Allow to connect the cluster
    - [**BC**] Change `Store.connect` to require `ConnectionConfiguration`
- [**BC**] Change `Store.tryToConnectWithReconnects` to use `async` instead of a thread
- [**BC**] Use `ILoggerFactory` instead of a log function
- [**BC**] Rename functions
    - `Configuration.toString` -> `Configuration.value`
    - `ConnectionConfiguration.toString` -> `ConnectionConfiguration.value`
- Add logging

## 3.0.0 - 2020-11-23
- Use .netcore 5.0

## 2.0.0 - 2020-11-20
- [**BC**] Change namespace to `Lmc.Aerospike`
- Use .netcore 3.1
- Update dependencies
- Add `AssemblyInfo`

## 1.2.0 - 2019-06-26
- Add lint

## 1.1.0 - 2019-06-06
- Fix store state log
- Add `Configuration.toString` function to allow formatting a configuration
- Add `ConnectionConfiguration.toString` function to allow formatting a connection configuration

## 1.0.0 - 2019-01-31
- Initial implementation
