#!/bin/bash
set -x

source="${BASH_SOURCE[0]}"

cd "$(dirname "$source")"
cd "../"
dotnet restore
dotnet watch --project FF1Blazorizer run

