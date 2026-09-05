FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MovieStreaming.slnx .
COPY src/MovieStreaming.Domain/MovieStreaming.Domain.csproj src/MovieStreaming.Domain/
COPY src/MovieStreaming.Infrastructure/MovieStreaming.Infrastructure.csproj src/MovieStreaming.Infrastructure/
COPY src/MovieStreaming.Api/MovieStreaming.Api.csproj src/MovieStreaming.Api/
RUN dotnet restore src/MovieStreaming.Api/MovieStreaming.Api.csproj

COPY src/ src/
RUN dotnet publish src/MovieStreaming.Api/MovieStreaming.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN adduser --disabled-password --gecos "" apiuser
USER apiuser

COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MovieStreaming.Api.dll"]
