@echo off

cd "./dataTomqttExtractor"
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish .\iot_agent.csproj
docker build -t dataextractor -f Dockerfile .
cd ..