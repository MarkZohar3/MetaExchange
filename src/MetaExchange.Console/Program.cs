// Program.cs (MetaExchange.Console)
//
// Usage:
//   dotnet run --project .\src\MetaExchange.Console\MetaExchange.Console.csproj -- <venueFilePath> [amount] [side]
//
// Example:
//   dotnet run --project .\src\MetaExchange.Console\MetaExchange.Console.csproj -- .\data\venues\order_books_data1 0.9 Buy

using MetaExchange.Application.BestExecution;
using MetaExchange.Domain.BestExecution;
using MetaExchange.Domain.Orders;
using Microsoft.Extensions.DependencyInjection;

if (args.Length == 0)
{
    Console.WriteLine("Usage: MetaExchange.ConsoleApp <venueFilePath> [amount] [side]");
    Console.WriteLine(@"Example: dotnet run --project .\src\MetaExchange.Console\MetaExchange.Console.csproj -- .\data\venues\order_books_data1 0.9 Buy");
    return;
}

var venueFilePath = args[0];

var requestedAmount = args.Length >= 2 && decimal.TryParse(args[1], out var amount) ? amount : 0.9m;

var side = OrderSide.Buy;
if (args.Length >= 3 && Enum.TryParse<OrderSide>(args[2], ignoreCase: true, out var parsedSide))
{
    side = parsedSide;
}

if (requestedAmount <= 0m)
{
    Console.WriteLine("requestedAmount must be > 0.");
    return;
}

var services = new ServiceCollection();
services.AddTransient<IBestExecutionService, BestExecutionService>();
var provider = services.BuildServiceProvider();
var service = provider.GetRequiredService<IBestExecutionService>();


BestExecutionPlan plan;
try
{
    plan = await service.PlanFromFileAsync(venueFilePath, side, requestedAmount);
}
catch (FileNotFoundException)
{
    Console.WriteLine($"File not found: {venueFilePath}");
    return;
}

Console.WriteLine($"Venues: {plan.Orders.Select(o => o.VenueId).Distinct().Count()}");
Console.WriteLine($"Request: {side} {requestedAmount} BTC");
Console.WriteLine();
Console.WriteLine($"Filled:   {plan.FilledBtc}/{plan.Amount} BTC");
Console.WriteLine($"TotalEUR: {plan.TotalEur}");

if (plan.Orders.Count == 0)
{
    Console.WriteLine("No executable orders (insufficient liquidity and/or balances).");
    return;
}

Console.WriteLine("Orders:");
foreach (var o in plan.Orders)
{
    Console.WriteLine($"  {o.VenueId} {o.Side} {o.Amount} BTC @ {o.LimitPriceEurPerBtc}");
}
