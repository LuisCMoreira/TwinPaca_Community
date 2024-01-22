@echo off

docker-compose down

docker rmi twinpack_frontend
docker rmi data_extractor:1.0.0
docker rmi data_manager:1.0.0
docker rmi twinapi:1.0.0
docker rmi eclipse-mosquitto
docker rmi mongo

REM If you want to remove all dangling images uncomment
REM docker image prune -f
