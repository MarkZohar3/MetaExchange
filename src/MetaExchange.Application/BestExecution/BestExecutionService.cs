using MetaExchange.Application.MarketData.Ingestion;
using MetaExchange.Domain.BestExecution;
using MetaExchange.Domain.Orders;
using MetaExchange.Domain.Venues;

namespace MetaExchange.Application.BestExecution;

public sealed class BestExecutionService : IBestExecutionService
{
    public BestExecutionPlan Plan(OrderSide side, decimal amount, IReadOnlyList<VenueSnapshot> venues)
        => BestExecutionPlanner.Plan(side, amount, venues);

    public BestExecutionPlan PlanFromFile(string venueFilePath, OrderSide side, decimal amount)
    {
        if (!File.Exists(venueFilePath))
        {
            throw new FileNotFoundException("Venue file not found.", venueFilePath);
        }

        var snapshots = OrderBookFileReader
            .ReadSnapshots(venueFilePath)
            .Select((parsed, index) => new VenueSnapshot(
                venueId: $"Venue-{index + 1}",
                book: parsed.Snapshot,
                balances: parsed.Balances))
            .ToArray();

        return Plan(side, amount, snapshots);
    }
}
