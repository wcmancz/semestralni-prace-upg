#!/bin/bash
mkdir -p ./bin

dotnet msbuild ./src/SpaceTraffic.sln -t:Rebuild -p:Configuration=Release -restore

cp -r ./src/bin/Release/net10.0/* ./bin/