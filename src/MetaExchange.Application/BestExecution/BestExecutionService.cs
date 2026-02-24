using MetaExchange.Application.MarketData.Ingestion;
using MetaExchange.Domain.BestExecution;
using MetaExchange.Domain.Orders;
using MetaExchange.Domain.Venues;

namespace MetaExchange.Application.BestExecution;

public sealed class BestExecutionService : IBestExecutionService
{
    public BestExecutionPlan Plan(OrderSide side, decimal amount, IReadOnlyList<VenueSnapshot> venues)
        => BestExecutionPlanner.Plan(side, amount, venues);

    public BestExecutionPlan PlanFromDirectory(string venuesDir, OrderSide side, decimal amount)
    {
        if (!Directory.Exists(venuesDir))
        {
            throw new DirectoryNotFoundException(venuesDir);
        }
        var files = Directory
            .EnumerateFiles(venuesDir)
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase);

        var snapshots = new List<VenueSnapshot>();

        foreach (var file in files)
        {
            var id = Path.GetFileNameWithoutExtension(file);
            var parsed = OrderBookFileReader.ReadSnapshots(file, maxLines: 1).FirstOrDefault();
            if (parsed is null)
            {
                continue;
            }

            snapshots.Add(new VenueSnapshot(
                venueId: id,
                book: parsed.Snapshot,
                balances: parsed.Balances));
        }

        return Plan(side, amount, snapshots);
    }
}
