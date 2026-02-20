namespace MetaExchange.Application.MarketData.DTOs;

using MetaExchange.Domain.OrderBooks;

public sealed class OrderBookSnapshotDto
{
    public decimal UnixTimeSeconds { get; init; }
    public DateTime AcqTime { get; init; }

    public List<OrderEntryDto> Bids { get; init; } = new();
    public List<OrderEntryDto> Asks { get; init; } = new();

    public OrderBookSnapshot ToDomain()
    {
        return new OrderBookSnapshot(
            UnixTimeSeconds,
            AcqTime,
            Bids.Select(e => new PriceLevel(e.Order.Price, e.Order.Amount)).ToList(),
            Asks.Select(e => new PriceLevel(e.Order.Price, e.Order.Amount)).ToList()
        );
    }
}