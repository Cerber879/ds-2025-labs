@echo off
echo Starting Valuator on port 5001...
cd ..\Valuator
start /B dotnet run --urls "http://0.0.0.0:5001"
echo Valuator (port 5001) started.

echo Starting Valuator on port 5002...
start /B dotnet run --urls "http://0.0.0.0:5002"
echo Valuator (port 5002) started.

echo Starting Redis using docker-compose...
cd ..\InfrastructureRedis
docker-compose up -d

echo Starting Nginx using docker-compose...
cd ..\nginx\conf
docker-compose up -d

echo Starting RabbitMQ using docker-compose...
cd ..\..\Valuator\RabbitMQ
docker-compose up -d 

echo Waiting for RabbitMQ to start...
:wait_rabbitmq
timeout /t 2 /nobreak >nul
curl -s http://localhost:15672 >nul 2>&1
if %errorlevel% neq 0 goto wait_rabbitmq

echo RabbitMQ is ready.

echo Starting RankCalculator...
cd ..\..\RankCalculator
start /B dotnet run
start /B dotnet run
echo RankCalculator started.

echo Starting EventsLogger instance 1...
cd ..\EventsLogger
start /B dotnet run > ..\EventsLogger\logs\eventslogger1.log 2>&1
echo EventsLogger instance 1 started.

echo Starting EventsLogger instance 2...
start /B dotnet run > ..\EventsLogger\logs\eventslogger2.log 2>&1
echo EventsLogger instance 2 started.

cd ..\scripts

echo All components successfully started.