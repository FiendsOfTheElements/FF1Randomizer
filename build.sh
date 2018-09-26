#!/bin/bash

set -ex

rm -f FF1Randomizer.zip FF1R.tar.gz

pushd FF1Randomizer
rm -rf Bin/Release
/c/Program\ Files\ \(x86\)/Microsoft\ Visual\ Studio/2017/Community/MSBuild/15.0/Bin/amd64/MSBuild.exe FF1Randomizer.csproj -p:Configuration=Release
cd Bin/Release
git rev-parse HEAD > version.txt
/c/Program\ Files/7-Zip/7z.exe a -r -tzip ../../../FF1Randomizer.zip
popd

pushd FF1R
rm -rf bin/Release/netcoreapp2.1/publish
dotnet restore
dotnet publish -c Release
cd bin/Release/netcoreapp2.1/publish
git rev-parse HEAD > version.txt
tar czf ff1r.tar.gz *
mv ff1r.tar.gz ../../../../..
popd

