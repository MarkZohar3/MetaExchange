// Program.cs (MetaExchange.Console)
//
// Usage:
//   dotnet run --project .\src\MetaExchange.Console\MetaExchange.Console.csproj -- <venuesDir> [requestedBtc] [side]
//
// Example:
//   dotnet run --project .\src\MetaExchange.Console\MetaExchange.Console.csproj -- .\data\venues 0.9 Buy

using MetaExchange.Application.BestExecution;
using MetaExchange.Domain.Orders;
using Microsoft.Extensions.DependencyInjection;

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
{
    side = parsedSide;
}

if (requestedBtc <= 0m)
{
    Console.WriteLine("requestedBtc must be > 0.");
    return;
}

var services = new ServiceCollection();
services.AddTransient<IBestExecutionService, BestExecutionService>();
var provider = services.BuildServiceProvider();
var service = provider.GetRequiredService<IBestExecutionService>();


BestExecutionPlan plan;
try
{
    plan = service.PlanFromDirectory(venuesDir, side, requestedBtc);
}
catch (DirectoryNotFoundException)
{
    Console.WriteLine($"Directory not found: {venuesDir}");
    return;
}

Console.WriteLine($"Venues: {plan.Orders.Select(o => o.VenueId).Distinct().Count()}");
Console.WriteLine($"Request: {side} {requestedBtc} BTC");
Console.WriteLine();
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