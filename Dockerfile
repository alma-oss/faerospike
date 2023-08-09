FROM dcreg.service.consul/dev/development-dotnet-core-sdk-common:7.0

# build scripts
COPY ./build.sh /library/
COPY ./build /library/build
COPY ./paket.dependencies /library/
COPY ./paket.references /library/
COPY ./paket.lock /library/

# sources
COPY ./Aerospike.fsproj /library/
COPY ./src /library/src

# others
COPY ./.git /library/.git
COPY ./.config /library/.config
COPY ./CHANGELOG.md /library/

WORKDIR /library

RUN \
    ./build.sh -t Build no-clean

CMD ["./build.sh", "-t", "Tests", "no-clean"]
