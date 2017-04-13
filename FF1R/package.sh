#~/bin/bash

rm bin/Release/netcoreapp1.1/publish/*
dotnet restore
dotnet publish -c Release
pushd bin/Release/netcoreapp1.1/publish
tar czf ff1r.tar.gz *
mv ff1r.tar.gz ../../../..
popd
