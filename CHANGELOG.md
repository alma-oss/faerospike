# Changelog

<!-- There is always Unreleased section on the top. Subsections (Add, Changed, Fix, Removed) should be Add as needed. -->
## Unreleased
- [**BC**] Update dependencies
    - Aerospike.Client ~> 4.1 (which requires Aerospike.Server ^5.0)
- [**BC**] Requires Qualified Access for Modules
- Add more connect functions
    - `Store.tryToConnect`
    - `Store.tryToConnectWithReconnects`
    - `Store.connectWithReconnects`

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
