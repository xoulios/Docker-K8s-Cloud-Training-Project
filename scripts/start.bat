@echo off
rem Starts the database and the service on a user-defined network.
rem The service container is created only after the database reports healthy.

setlocal
call "%~dp0env.bat"

set /a MAX_ATTEMPTS=90

docker image inspect %IMAGE_NAME% >nul 2>&1
if errorlevel 1 (
    echo [start] Image %IMAGE_NAME% is missing. Run setup.bat first.
    endlocal
    exit /b 1
)

docker volume inspect %DB_VOLUME% >nul 2>&1
if errorlevel 1 (
    echo [start] Volume %DB_VOLUME% is missing. Run setup.bat first.
    endlocal
    exit /b 1
)

echo [start] Removing leftover containers
docker stop %API_CONTAINER% >nul 2>&1
docker rm %API_CONTAINER% >nul 2>&1
docker stop %DB_CONTAINER% >nul 2>&1
docker rm %DB_CONTAINER% >nul 2>&1

echo [start] Ensuring network %NETWORK_NAME%
docker network inspect %NETWORK_NAME% >nul 2>&1 || docker network create %NETWORK_NAME% >nul
if errorlevel 1 goto fail

echo [start] Starting database container %DB_CONTAINER%
docker run --detach ^
    --name %DB_CONTAINER% ^
    --network %NETWORK_NAME% ^
    --volume %DB_VOLUME%:/var/opt/mssql ^
    --env ACCEPT_EULA=Y ^
    --env MSSQL_PID=Developer ^
    --env MSSQL_SA_PASSWORD=%MSSQL_SA_PASSWORD% ^
    --health-cmd "%SQLCMD% -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -Q 'SELECT 1'" ^
    --health-interval 5s ^
    --health-timeout 5s ^
    --health-start-period 10s ^
    --health-retries 3 ^
    --memory 2g ^
    --restart unless-stopped ^
    --security-opt no-new-privileges ^
    %DB_IMAGE% >nul
if errorlevel 1 goto fail

echo [start] Waiting for the database to become healthy
set /a ATTEMPT=0

:wait_db
set /a ATTEMPT+=1
for /f "tokens=*" %%h in ('docker inspect --format "{{.State.Health.Status}}" %DB_CONTAINER% 2^>nul') do set "DB_HEALTH=%%h"
if /i "%DB_HEALTH%"=="healthy" goto db_ready
if %ATTEMPT% GEQ %MAX_ATTEMPTS% goto db_timeout
echo         %DB_HEALTH% (%ATTEMPT%/%MAX_ATTEMPTS%)
ping -n 3 127.0.0.1 >nul
goto wait_db

:db_timeout
echo [start] Database did not become healthy in time. Logs:
docker logs --tail 30 %DB_CONTAINER%
goto fail

:db_ready
echo [start] Database is healthy. Applying db\init.sql
docker cp "%PROJECT_ROOT%\db\init.sql" %DB_CONTAINER%:/tmp/init.sql >nul
if errorlevel 1 goto fail
docker exec %DB_CONTAINER% /bin/bash -c "%SQLCMD% -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -b -i /tmp/init.sql"
if errorlevel 1 goto fail

echo [start] Starting service container %API_CONTAINER%
docker run --detach ^
    --name %API_CONTAINER% ^
    --network %NETWORK_NAME% ^
    --publish %API_PORT%:8080 ^
    --env "ConnectionStrings__MovieDb=%DB_CONNECTION%" ^
    --memory 512m ^
    --restart unless-stopped ^
    --security-opt no-new-privileges ^
    --cap-drop ALL ^
    %IMAGE_NAME% >nul
if errorlevel 1 goto fail

echo [start] Waiting for the service to answer on port %API_PORT%
set /a ATTEMPT=0

:wait_api
set /a ATTEMPT+=1
curl --silent --fail --max-time 3 --output nul http://localhost:%API_PORT%/api/movies && goto api_ready
if %ATTEMPT% GEQ 40 goto api_timeout
ping -n 3 127.0.0.1 >nul
goto wait_api

:api_timeout
echo [start] Service did not answer in time. Logs:
docker logs --tail 30 %API_CONTAINER%
goto fail

:api_ready
echo [start] Running. API: http://localhost:%API_PORT%/api/movies
endlocal
exit /b 0

:fail
echo [start] FAILED.
endlocal
exit /b 1
