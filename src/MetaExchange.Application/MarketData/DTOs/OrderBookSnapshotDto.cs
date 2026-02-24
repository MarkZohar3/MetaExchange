namespace MetaExchange.Application.MarketData.DTOs;

using MetaExchange.Domain.OrderBooks;
using MetaExchange.Domain.Venues;

public sealed class OrderBookSnapshotDto
{
    public DateTime AcqTime { get; init; }

    public List<OrderEntryDto> Bids { get; init; } = new();
    public List<OrderEntryDto> Asks { get; init; } = new();

    public OrderBookSnapshot ToDomain(VenueBalances venueBalances)
    {
        ArgumentNullException.ThrowIfNull(Bids);
        ArgumentNullException.ThrowIfNull(Asks);

        var bids = Bids.Select(e =>
        {
            if (e is null)
            {
                throw new ArgumentException("Bid list contains a null element");
            }
            if (e.Order.Price <= 0m || e.Order.Amount <= 0m)
            {
                throw new ArgumentException("PriceLevel values must be positive");
            }
            return new PriceLevel(e.Order.Price, e.Order.Amount);
        }).ToList();

        var asks = Asks.Select(e =>
        {
            if (e is null)
            {
                throw new ArgumentException("Ask list contains a null element");
            }
            if (e.Order.Price <= 0m || e.Order.Amount <= 0m)
            {
                throw new ArgumentException("PriceLevel values must be positive");
            }
            return new PriceLevel(e.Order.Price, e.Order.Amount);
        }).ToList();

        return new OrderBookSnapshot(
            venueBalances,
            AcqTime,
            bids,
            asks
        );
    }
}