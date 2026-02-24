using MetaExchange.Application.MarketData.Ingestion;
using MetaExchange.Domain.Orders;

namespace MetaExchange.Application.BestExecution;

public sealed class BestExecutionService : IBestExecutionService
{
    private const decimal DefaultEur = 50_000m;
    private const decimal DefaultBtc = 2m;

    public BestExecutionPlan Plan(OrderSide side, decimal requestedBtc, IReadOnlyList<VenueSnapshot> venues)
        => BestExecutionPlanner.Plan(side, requestedBtc, venues);

    public BestExecutionPlan PlanFromDirectory(string venuesDir, OrderSide side, decimal requestedBtc)
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
            var src = new FileMarketDataSource(file);
            var book = src.ReadSnapshots(maxLines: 1).FirstOrDefault();
            if (book is null)
            {
                continue;
            }
                

            snapshots.Add(new VenueSnapshot(
                VenueId: id,
                Book: book,
                Balances: new VenueBalances(DefaultEur, DefaultBtc)));
        }

        return Plan(side, requestedBtc, snapshots);
    }
}
