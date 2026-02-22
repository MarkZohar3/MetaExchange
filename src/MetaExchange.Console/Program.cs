// Program.cs (MetaExchange.Console)
//
// Usage:
//   dotnet run --project .\src\MetaExchange.Console\MetaExchange.Console.csproj -- <venuesDir> [requestedBtc] [side]
//
// Example:
//   dotnet run --project .\src\MetaExchange.Console\MetaExchange.Console.csproj -- .\data\venues 0.9 Buy

using MetaExchange.Application.BestExecution;
using MetaExchange.Application.MarketData;
using MetaExchange.Application.MarketData.Ingestion;

if (args.Length == 0)
{
    Console.WriteLine("Usage: MetaExchange.ConsoleApp <venuesDir> [requestedBtc] [side]");
    Console.WriteLine(@"Example: dotnet run --project .\src\MetaExchange.Console\MetaExchange.Console.csproj -- .\data\venues 0.9 Buy");
    return;
}

var venuesDir = args[0];

var requestedBtc = args.Length >= 2 && decimal.TryParse(args[1], out var amount) ? amount : 0.9m;

var side = OrderSide.Buy;
if (args.Length >= 3 && Enum.TryParse<OrderSide>(args[2], ignoreCase: true, out var parsedSide))
    side = parsedSide;

if (!Directory.Exists(venuesDir))
{
    Console.WriteLine($"Directory not found: {venuesDir}");
    return;
}

if (requestedBtc <= 0m)
{
    Console.WriteLine("requestedBtc must be > 0.");
    return;
}

// Venue files (one file per venue)
var venueFiles = Directory.EnumerateFiles(venuesDir)
    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
    .ToList();

if (venueFiles.Count == 0)
{
    Console.WriteLine($"No venue files found in: {venuesDir}");
    return;
}

// Temporary default balances (same for all venues)
const decimal defaultEur = 50_000m;
const decimal defaultBtc = 2m;

var venues = new List<VenueSnapshot>(venueFiles.Count);

foreach (var file in venueFiles)
{
    var venueId = Path.GetFileNameWithoutExtension(file);

    IMarketDataSource source = new FileMarketDataSource(file);

    // We take the latest available snapshot per venue.
    var book = source.ReadSnapshots(maxLines: 1).FirstOrDefault();
    if (book is null)
    {
        Console.WriteLine($"Warning: {venueId} has no snapshots (file: {file}). Skipping.");
        continue;
    }

    venues.Add(new VenueSnapshot(
        VenueId: venueId,
        Book: book,
        Balances: new VenueBalances(Eur: defaultEur, Btc: defaultBtc)));
}

if (venues.Count == 0)
{
    Console.WriteLine("No usable venues loaded.");
    return;
}

Console.WriteLine($"Venues: {venues.Count}");
Console.WriteLine($"Request: {side} {requestedBtc} BTC");
Console.WriteLine();

var plan = BestExecutionPlanner.Plan(side, requestedBtc, venues);

Console.WriteLine($"Filled:   {plan.FilledBtc}/{plan.RequestedBtc} BTC");
Console.WriteLine($"TotalEUR: {plan.TotalEur}");

if (plan.Orders.Count == 0)
{
    Console.WriteLine("No executable orders (insufficient liquidity and/or balances).");
    return;
}

Console.WriteLine("Orders:");
foreach (var o in plan.Orders)
{
    Console.WriteLine($"  {o.VenueId} {o.Side} {o.QuantityBtc} BTC @ {o.LimitPriceEurPerBtc}");
}