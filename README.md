# MetaExchange

This repository contains a small **meta-exchange** proof‑of‑concept implemented in .NET 10.0.  It was created for an interview / coding task and demonstrates:

* **Part 1 – Best execution algorithm** – given a set of order books coming from multiple venues, a requested buy or sell amount of BTC and per‑venue balances, compute a list of child orders that achieves the best price for the user while respecting each venue's EUR/BTC balance constraints.  Buyer orders consume EUR and pick cheapest asks; seller orders consume BTC and pick highest bids.
* **Part 2 – Web service** – the same planner is exposed as an HTTP API using a minimal ASP.NET Core (Kestrel) service with Swagger/OpenAPI.

The solution is split into three projects:

* `MetaExchange.Domain` – simple domain types (`OrderBookSnapshot`, `PriceLevel`, `OrderSide`, etc.).
* `MetaExchange.Application` – core logic: market‑data ingestion, the best‑execution planner and a service wrapper used by both console and API.
* `MetaExchange.Api` – ASP.NET Core application exposing `/best-execution/plan`.
* `MetaExchange.Console` – command‑line frontend for quick experimentation.

A small xUnit test project (`MetaExchange.Application.Tests`) validates the planner behaviour.

---

## Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download)
* `docker` / `docker-compose` for container builds 

## Data format

Order‑book snapshots are expected as JSON lines; each line is deserialized into `OrderBookSnapshotDto`.  The `data/venues` directory contains sample files used by the console app and the API when run with the default configuration.

A single snapshot looks like the examples included in the `data` folder.

## Console application

The console application is a simple entry point that reads the order books from a directory and prints a plan to stdout.

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

## Tests

The unit tests live under `tests/MetaExchange.Application.Tests`.  To execute them:

```powershell
dotnet test tests/MetaExchange.Application.Tests/MetaExchange.Application.Tests.csproj
```

The test suite covers the planner; extend it if you add new business logic.

## Docker

A `docker-compose.yml` is provided to build and run both components with the sample data directory mounted read‑only.

```bash
# build images
docker-compose build

# start services
docker-compose up
```

The API will be available on `http://localhost:8080` and Swagger UI at `http://localhost:8080/swagger` when the environment variable `ASPNETCORE_ENVIRONMENT` is `Development`.

You can also run just the console image to verify the algorithm in a container:

```bash
docker run --rm -v $(pwd)/data:/data:ro metaexchange_console /data/venues 1.5 Sell
```

(or adjust the image name/tag as output by `docker-compose build`).

## Project structure overview

```
MetaExchange.sln
├── src/
│   ├── MetaExchange.Api/        # web service
│   ├── MetaExchange.Application/ # business logic & market data
│   ├── MetaExchange.Console/     # CLI helper
│   └── MetaExchange.Domain/      # shared domain types
└── tests/
    └── MetaExchange.Application.Tests/ # xUnit tests
```
