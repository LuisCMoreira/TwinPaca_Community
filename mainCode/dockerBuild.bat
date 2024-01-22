@echo off

cd "./dataTomqttExtractor"
dotnet publish --os linux --arch x64 /t:PublishContainer -o Release .\iot_agent.csproj
cd ..

cd "./dataManager"
dotnet publish --os linux --arch x64 /t:PublishContainer -o Release .\dataManager.csproj
cd ..

cd "./twinAPI"
dotnet publish --os linux --arch x64 /t:PublishContainer -o Release .\twinAPI.csproj
cd ..

cd "./unityPJ/twinSample_webGLbuild"
docker build -t unity_sample .
cd ../..

cd "./frontEnd"
docker build -t twinpack_frontend .
cd ..

docker-compose up -d