@echo off
rem Stops and removes the containers and the network.
rem The database volume is kept on purpose so data survives a restart.

setlocal
call "%~dp0env.bat"

echo [stop] Stopping containers
docker stop %API_CONTAINER% >nul 2>&1
docker stop %DB_CONTAINER% >nul 2>&1

echo [stop] Removing containers
docker rm %API_CONTAINER% >nul 2>&1
docker rm %DB_CONTAINER% >nul 2>&1

echo [stop] Removing network %NETWORK_NAME%
docker network rm %NETWORK_NAME% >nul 2>&1

echo [stop] Done. Volume %DB_VOLUME% was kept.
echo [stop] To discard the data as well: docker volume rm %DB_VOLUME%
endlocal
exit /b 0
