#!/bin/sh

# Run Release build and Run Tests, Coverlet to record result and OpenCover code-coverage
echo "Running tests..."
dotnet test /p:Configuration=Release ./Dunk.Tools.Data.Test/Dunk.Tools.Data.Test.csproj  --framework netcoreapp3.1 --logger:trx /p:CollectCoverage=true /p:CoverletOutputFormat=opencover