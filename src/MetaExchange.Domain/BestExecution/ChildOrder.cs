using MetaExchange.Domain.Orders;

namespace MetaExchange.Domain.BestExecution;


public sealed record ChildOrder
{
    public string VenueId { get; }
    public OrderSide Side { get; }
    public decimal Amount { get; }
    public decimal LimitPriceEurPerBtc { get; }

    public ChildOrder(string venueId, OrderSide side, decimal amount, decimal limitPriceEurPerBtc)
    {
        if (string.IsNullOrWhiteSpace(venueId))
        {
            throw new ArgumentException("VenueId must be provided", nameof(venueId));
        }
        if (amount <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");
        }
        if (limitPriceEurPerBtc <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(limitPriceEurPerBtc), "LimitPriceEurPerBtc must be positive");
        }

        VenueId = venueId;
        Side = side;
        Amount = amount;
        LimitPriceEurPerBtc = limitPriceEurPerBtc;
    }
}
