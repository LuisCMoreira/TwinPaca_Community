@echo off

cd "./dataTomqttExtractor"
dotnet publish --os linux --arch x64 /t:PublishContainer -o Release .\iot_agent.csproj
cd ..

cd "./dataManager"
dotnet publish --os linux --arch x64 /t:PublishContainer -o Release .\dataManager.cspro
cd ..

cd "./frontEnd"
docker build -t twinpack_frontend .
cd ..

docker run -p 502:502 -p 135:135 -p 1883:1883 docker-net-data_extractor:1.0.0
docker run -p 27017:27017 -p 1883:1883 docker-net-data_manager:1.0.0
docker run -p 3001:3000 -d twinpack_frontend:1.0.0

docker-compose up -d