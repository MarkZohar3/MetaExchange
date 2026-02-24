using MetaExchange.Application.MarketData.Ingestion;
using MetaExchange.Domain.BestExecution;
using MetaExchange.Domain.Orders;
using MetaExchange.Domain.Venues;

namespace MetaExchange.Application.BestExecution;

public sealed class BestExecutionService : IBestExecutionService
{
    public Task<BestExecutionPlan> PlanAsync(OrderSide side, decimal amount, IReadOnlyList<VenueSnapshot> venues)
        => Task.FromResult(BestExecutionPlanner.Plan(side, amount, venues));

    public async Task<BestExecutionPlan> PlanFromFileAsync(string venueFilePath, OrderSide side, decimal amount, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(venueFilePath))
        {
            throw new FileNotFoundException("Venue file not found.", venueFilePath);
        }

        var parsedSnapshots = await OrderBookFileReader.ReadSnapshotsAsync(venueFilePath, cancellationToken);

        var snapshots = parsedSnapshots
            .Select((parsed, index) => new VenueSnapshot(
                venueId: $"Venue-{index + 1}",
                book: parsed.Snapshot,
                balances: parsed.Balances))
            .ToArray();

        return await PlanAsync(side, amount, snapshots);
    }
}
