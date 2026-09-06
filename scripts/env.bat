@echo off
rem Shared configuration for setup / start / stop.

set "IMAGE_NAME=moviestreaming-api:1.0"
set "DB_IMAGE=mcr.microsoft.com/mssql/server:2022-latest"
set "NETWORK_NAME=moviestreaming-net"
set "DB_VOLUME=moviestreaming-db-data"
set "DB_CONTAINER=moviestreaming-db"
set "API_CONTAINER=moviestreaming-api"
set "API_PORT=8080"
set "DB_NAME=MovieStreamingDb"
set "APP_DB_USER=movieapi"
set "APP_DB_PASSWORD=Movie_Api_2026!"
set "SQLCMD=/opt/mssql-tools18/bin/sqlcmd"
set "PROJECT_ROOT=%~dp0.."

if "%MSSQL_SA_PASSWORD%"=="" set "MSSQL_SA_PASSWORD=Str0ng_SA_Pass_2026"

set "DB_CONNECTION=Server=%DB_CONTAINER%,1433;Database=%DB_NAME%;User Id=%APP_DB_USER%;Password=%APP_DB_PASSWORD%;TrustServerCertificate=True;"
