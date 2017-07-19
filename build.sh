#!/usr/bin/env bash

#exit if any command fails
set -e

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

dotnet restore ./src/Invio.Extensions.Reflection
dotnet restore ./test/Invio.Extensions.Reflection.Tests

dotnet test ./test/Invio.Extensions.Reflection.Tests/Invio.Extensions.Reflection.Tests.csproj -c Release -f netcoreapp1.0

dotnet pack ./src/Invio.Extensions.Reflection -c Release -o ../../artifacts
