#!/bin/sh
set -e

cd api
cd Version/src/Version
dotnet lambda package --framework netcoreapp2.1 -o ../../../artifacts/Version.zip
