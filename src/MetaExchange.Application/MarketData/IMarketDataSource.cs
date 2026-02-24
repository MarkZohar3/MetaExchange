namespace MetaExchange.Application.MarketData;

using MetaExchange.Domain.OrderBooks;

public interface IMarketDataSource
{
    IEnumerable<OrderBookSnapshot> ReadSnapshots();
}