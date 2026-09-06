@echo off
rem Builds the service image and creates the database volume.

setlocal
call "%~dp0env.bat"

echo [setup] Building service image %IMAGE_NAME%
docker build --tag %IMAGE_NAME% "%PROJECT_ROOT%"
if errorlevel 1 goto fail

echo [setup] Pulling database image %DB_IMAGE%
docker pull %DB_IMAGE%
if errorlevel 1 goto fail

echo [setup] Creating volume %DB_VOLUME%
docker volume create %DB_VOLUME% >nul
if errorlevel 1 goto fail

echo [setup] Ready. Run start.bat next.
endlocal
exit /b 0

:fail
echo [setup] FAILED.
endlocal
exit /b 1
