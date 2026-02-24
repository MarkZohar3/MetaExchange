# MetaExchange

This repository contains a small **meta-exchange** proof‑of‑concept implemented in .NET 10.0. 


## Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download)
* `docker` / `docker-compose` for container builds 

## Data format

The `data/venues` directory contains sample files used by the console app and the API when run with the default configuration.

A single snapshot looks like the examples included in the `data` folder.

## Console application

The console application is a simple entry point that reads the order books from a file and prints a plan to stdout.

### Build & run

```powershell
# from the repo root
dotnet run --project MetaExchange.Console.csproj -- <venueFile> [requestedamountBtc] [side]
```

Example:

```powershell
dotnet run --project .\src\MetaExchange.Console\MetaExchange.Console.csproj -- .\data\venues\order_books_data1 0.9 Buy
```

If `amount` or `side` are omitted, the defaults are `0.9` and `Buy`.

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
# configure environment variable point at your venues file
$env:VenuesDirectory = "..\\..\\data\\venues\\order_books_data1"
dotnet run
```

Then browse to the Swagger UI using the host/port shown in the console output

#### POST /best-execution/plan

Body example:

```json
{
  "side": "Buy",
  "amount": 1.2
}
```

Response:

```json
{
  "side": "Buy",
  "amount": 1.2,
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

# start all services
docker-compose up

#start api
docker compose up -d api 

#start console with default parameters
docker compose run --rm console

#start console with custom parameters (example)
docker compose run --rm console /data/venues/order_books_data1 0.5 Buy
```

The API will be available on `http://localhost:8080` and Swagger UI at `http://localhost:8080/swagger`.
