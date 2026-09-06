FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props .
COPY src/MovieStreaming.Domain/MovieStreaming.Domain.csproj src/MovieStreaming.Domain/
COPY src/MovieStreaming.Infrastructure/MovieStreaming.Infrastructure.csproj src/MovieStreaming.Infrastructure/
COPY src/MovieStreaming.Api/MovieStreaming.Api.csproj src/MovieStreaming.Api/
RUN dotnet restore src/MovieStreaming.Api/MovieStreaming.Api.csproj

COPY src/ src/
RUN dotnet publish src/MovieStreaming.Api/MovieStreaming.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app .

USER $APP_UID
EXPOSE 8080

# The runtime image ships no curl or wget, so the probe uses the /dev/tcp device of bash.
HEALTHCHECK --interval=15s --timeout=5s --start-period=20s --retries=3 \
    CMD ["/bin/bash", "-c", "exec 3<>/dev/tcp/127.0.0.1/8080 && printf 'GET /health/live HTTP/1.1\r\nHost: localhost\r\nConnection: close\r\n\r\n' >&3 && head -1 <&3 | grep -q '200 OK'"]

ENTRYPOINT ["dotnet", "MovieStreaming.Api.dll"]
