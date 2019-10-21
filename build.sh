#!/bin/sh
set -e

cd web
npm run build

cd -
cd api/Version/src/Version
dotnet lambda package --framework netcoreapp2.1 -o ../../../artifacts/Version.zip

cd -
cd api/Messages/src/Messages
dotnet lambda package --framework netcoreapp2.1 -o ../../../artifacts/Messages.zip

cd -
cd api/Ingestion/src/Ingestion
dotnet lambda package --framework netcoreapp2.1 -o ../../../artifacts/Ingestion.zip
