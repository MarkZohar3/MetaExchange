namespace MetaExchange.Application.MarketData.Ingestion;

using MetaExchange.Application.MarketData;
using MetaExchange.Domain.OrderBooks;

public sealed class FileMarketDataSource : IMarketDataSource
{
    private readonly string _filePath;

    public FileMarketDataSource(string filePath)
    {
        _filePath = filePath;
    }

    public IEnumerable<OrderBookSnapshot> ReadSnapshots(int? maxLines = null)
    {
        foreach (var parsed in OrderBookFileReader.ReadSnapshots(_filePath, maxLines))
            yield return parsed.Snapshot;
    }
}