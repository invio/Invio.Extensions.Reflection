#!/usr/bin/env bash

#exit if any command fails
set -e

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

dotnet restore

# Ideally we would use the 'dotnet test' command to test netcoreapp and net46 so restrict for now
# but this currently doesn't work due to https://github.com/dotnet/cli/issues/3073 so restrict to netcoreapp

dotnet test ./test/Invio.Extensions.Reflection.Tests -c Release -f netcoreapp1.0

# Instead, run directly with mono for the full .net version
dotnet build ./test/Invio.Extensions.Reflection.Tests -c Release -f net46

mono \
./test/Invio.Extensions.Reflection.Tests/bin/Release/net46/*/dotnet-test-xunit.exe \
./test/Invio.Extensions.Reflection.Tests/bin/Release/net46/*/Invio.Extensions.Reflection.Tests.dll

revision=${TRAVIS_JOB_ID:=1}
revision=$(printf "%04d" $revision)

dotnet pack ./src/Invio.Extensions.Reflection -c Release -o ./artifacts
