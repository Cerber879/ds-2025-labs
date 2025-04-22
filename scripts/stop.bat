@echo off

echo Stopping Valuator on port 5001
echo Searching for a process using port 5001...
for /F "tokens=5" %%A in ('netstat -ano ^| find "5001"') do (
    echo Stopping web application on port 5001 with PID: %%A...
    taskkill /PID %%A /F > nul 2>&1
)
echo Valuator on port 5001 stopped.

echo Stopping Valuator on port 5002
echo Searching for a process using port 5002...
for /F "tokens=5" %%A in ('netstat -ano ^| find "5002"') do (
    echo Stopping Valuator on port 5002 with PID: %%A...
    taskkill /PID %%A /F > nul 2>&1
)
echo Valuator on port 5002 stopped.

echo Stopping Nginx using docker-compose...
cd ..\nginx\conf
docker-compose down
echo Nginx stopped.

echo Stopping Redis using docker-compose...
cd ..\..\InfrastructureRedis
docker-compose down
echo Redis stopped.

echo Stopping RabbitMQ container...
docker stop rabbitmq
docker rm rabbitmq
echo RabbitMQ stopped.

echo Stopping RankCalculator...
echo Searching for RankCalculator process...
for /F "tokens=2" %%A in ('tasklist ^| find "RankCalculator"') do (
    echo Stopping RankCalculator with PID: %%A...
    taskkill /PID %%A /F > nul 2>&1
)
echo RankCalculator stopped.

echo Stopping EventsLogger instance 1...
echo Searching for EventsLogger instance 1 process...
for /F "tokens=2" %%A in ('tasklist ^| find "EventsLogger"') do (
    echo Stopping EventsLogger instance 1 with PID: %%A...
    taskkill /PID %%A /F > nul 2>&1
)
echo EventsLogger instance 1 stopped.

echo Stopping EventsLogger instance 2...
echo Searching for EventsLogger instance 2 process...
for /F "tokens=2" %%A in ('tasklist ^| find "EventsLogger"') do (
    echo Stopping EventsLogger instance 2 with PID: %%A...
    taskkill /PID %%A /F > nul 2>&1
)
echo EventsLogger instance 2 stopped.

cd ..\scripts

echo All components successfully stopped.