FROM dcreg.service.consul/prod/development-dotnet-core-sdk-common:latest

# build scripts
COPY ./fake.sh /faerospike/
COPY ./build.fsx /faerospike/
COPY ./paket.dependencies /faerospike/
COPY ./paket.references /faerospike/
COPY ./paket.lock /faerospike/

# sources
COPY ./Aerospike.fsproj /faerospike/
COPY ./src /faerospike/src

WORKDIR /faerospike

RUN \
    ./fake.sh build target Build no-clean

CMD ["./fake.sh", "build", "target", "Tests", "no-clean"]
