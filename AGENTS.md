# AGENTS.md — Alma.Aerospike

## Project Purpose

F# library providing a functional wrapper around the Aerospike .NET client for key-value store operations. Supports connecting to Aerospike clusters and storing/retrieving state data. Published as NuGet package `Alma.Aerospike`.

## Tech Stack

- **Language:** F# (.NET 10)
- **Framework:** .NET SDK library
- **Package management:** Paket
- **Build system:** FAKE (F# Make) via `build.sh`
- **Linting:** fsharplint
- **CI/CD:** GitHub Actions
- **Key dependencies:** `FSharp.Core ~> 10.0`, `Aerospike.Client ~> 6.0`, `Feather.ErrorHandling ~> 2.0`, `Microsoft.Extensions.Logging.Abstractions ~> 10.0`

## Commands

```bash
# Install dependencies
dotnet tool restore && dotnet paket install

# Build
./build.sh build

# Run tests
./build.sh -t tests

# Lint
dotnet fsharplint lint Aerospike.fsproj
```

## Project Structure

```
faerospike/
├── Aerospike.fsproj            # Main project (PackageId: Alma.Aerospike, v11.0.0)
├── AssemblyInfo.fs             # Auto-generated
├── src/
│   ├── Types.fs                # Connection and configuration types
│   └── Store.fs                # Store operations (connect, storeState)
├── build/
│   ├── build.fsproj            # FAKE build project
│   └── ...
├── build.sh                    # Build entry script
├── paket.dependencies          # Top-level dependencies
├── paket.references            # FSharp.Core, Aerospike.Client, Feather.ErrorHandling, Logging.Abstractions
├── global.json                 # .NET SDK 10.0.0
├── fsharplint.json             # Lint configuration
├── CHANGELOG.md
└── .github/workflows/
    ├── tests.yaml              # Tests on PRs and nightly
    ├── pr-check.yaml           # Fixup commit blocker, ShellCheck
    └── publish.yaml            # NuGet publish on tags
```

## Architecture

Pure library exposing:

- **Types** — `ConnectionConfiguration` (host, port), `Configuration` (namespace, set name)
- **Store** — `connect` (creates disposable client), `storeState` (stores key-value pairs)
- Uses `Microsoft.Extensions.Logging.Abstractions` for logging interface

## Build System (FAKE)

Standard library target chain: `Clean → AssemblyInfo → Build → Lint → Tests → Release → Publish`

## CI/CD

- **tests.yaml** — runs on PRs and nightly
- **pr-check.yaml** — blocks fixup commits, runs ShellCheck
- **publish.yaml** — publishes to NuGet on semver tags

## Release Process

1. Increment `<Version>` in `Aerospike.fsproj`
2. Update `CHANGELOG.md`
3. Commit, tag with version, push

## Conventions

- Functional style with baked-in dependencies (partial application pattern)
- Connection is `IDisposable` — use `use` binding
- `Feather.ErrorHandling` for Result patterns
- Compile order in `.fsproj` matters

## Pitfalls

- **No tests** — no test project exists currently
- **No Docker** — pure library; consuming services need their own Aerospike instance
- **Aerospike.Client pinned to ~> 6.0** — version 7.0+ has a known protobuf bug (see `paket.dependencies` comment)
- **Paket, not NuGet CLI** — use `dotnet paket install`
