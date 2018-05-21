#!/bin/bash

rm bin/Release/netcoreapp2.0/publish/*
dotnet restore
dotnet publish -c Release
pushd bin/Release/netcoreapp2.0/publish
tar czf ff1r.tar.gz *
mv ff1r.tar.gz ../../../..
popd
