# MetaExchange

This repository contains a small **meta-exchange** proof‑of‑concept implemented in .NET 10.0. 


## Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download)
* `docker` / `docker-compose` for container builds 

## Data format

The `data/venues` directory contains sample files used by the console app and the API when run with the default configuration.

A single snapshot looks like the examples included in the `data` folder.

## Console application

The console application is a simple entry point that reads the order books from a directory and prints a plan to stdout.
Every file is considered as a separate venue.

### Build & run

```powershell
# from the repo root
cd src/MetaExchange.Console
dotnet run --project MetaExchange.Console.csproj -- <venuesDir> [requestedBtc] [side]
```

Example:

```powershell
cd d:\Documents\Projects\MetaExchange
dotnet run --project .\src\MetaExchange.Console\MetaExchange.Console.csproj -- .\data\venues 0.9 Buy
```

If `requestedBtc` or `side` are omitted, the defaults are `0.9` and `Buy`.

### Sample output

```
Venues: 3
Request: Buy 0.9 BTC

Filled:   0.900000000000000000 BTC
TotalEUR: 27000.00
Orders:
  venue1 Buy 0.5 BTC @ 30000
  venue2 Buy 0.4 BTC @ 32000
```

Balance constraints and available liquidity will affect the filled amount.

## Web API

The API is located in `src/MetaExchange.Api` and listens on port **8080** by default.  Swagger is available when running in the Development environment.

### Running locally

```powershell
cd src/MetaExchange.Api
# configure environment variable or appsettings to point at your venues directory
$env:VenuesDirectory = "D:\\Documents\\Projects\\MetaExchange\\data\\venues"
dotnet run
```

Then browse to [https://localhost:5001/swagger](https://localhost:5001/swagger) (or http://localhost:5000) to exercise the endpoint.

#### POST /best-execution/plan

Body example:

```json
{
  "side": "Buy",
  "requestedBtc": 1.2
}
```

Response:

```json
{
  "side": "Buy",
  "requestedBtc": 1.2,
  "filledBtc": 1.2,
  "totalEur": 36000.0,
  "orders": [
    { "venue": "venueA", "side": "Buy", "price": 30000, "amountBtc": 1.0, "totalEur": 30000 },
    { "venue": "venueB", "side": "Buy", "price": 6000, "amountBtc": 0.2, "totalEur": 12000 }
  ]
}
```

The API interprets `side` case‑insensitively; only `Buy` or `Sell` are allowed.


## Docker

A `docker-compose.yml` is provided to build and run both components with the sample data directory mounted read‑only.

```bash
# build images
docker-compose build

# start services
docker-compose up
```

The API will be available on `http://localhost:8080` and Swagger UI at `http://localhost:8080/swagger` when the environment variable `ASPNETCORE_ENVIRONMENT` is `Development`.
