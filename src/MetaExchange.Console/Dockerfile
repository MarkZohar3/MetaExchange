# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.sln ./

COPY src/MetaExchange.Console/*.csproj src/MetaExchange.Console/
COPY src/MetaExchange.Application/*.csproj src/MetaExchange.Application/
COPY src/MetaExchange.Domain/*.csproj src/MetaExchange.Domain/

RUN dotnet restore ./src/MetaExchange.Console/MetaExchange.Console.csproj

COPY . .
RUN dotnet publish ./src/MetaExchange.Console/MetaExchange.Console.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MetaExchange.Console.dll"]