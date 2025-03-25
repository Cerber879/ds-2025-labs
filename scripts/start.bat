@echo off
echo Starting Valuator on port 5001...
cd ..\Valuator
start /B dotnet run --urls "http://0.0.0.0:5001"
echo Valuator (port 5001) started.

echo Starting Valuator on port 5002...
start /B dotnet run --urls "http://0.0.0.0:5002"
echo Valuator (port 5002) started.

echo Starting Nginx using docker-compose...
docker-compose up -d

echo Starting Redis using docker-compose...
cd ..\nginx\conf
docker-compose up -d 

cd ..\..\scripts

echo All components successfully started.