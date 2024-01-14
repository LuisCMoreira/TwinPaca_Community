@echo off
cd "./dataManager"
start cmd /k dotnet run
cd ..

cd "./localAPI"
start cmd /k dotnet run
cd ..

cd "./frontEnd"
start cmd /k npx nodemon app.js
cd ..

cd "./dataTomqttExtractor"
start cmd /k dotnet run
cd ..