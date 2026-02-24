using MetaExchange.Domain.Orders;

namespace MetaExchange.Domain.BestExecution;

public sealed record ChildOrder
{
    public string VenueId { get; }
    public OrderSide Side { get; }
    public decimal QuantityBtc { get; }
    public decimal LimitPriceEurPerBtc { get; }

    public ChildOrder(string venueId, OrderSide side, decimal quantityBtc, decimal limitPriceEurPerBtc)
    {
        if (quantityBtc < 0)
        {
            throw new ArgumentException("QuantityBtc must be non-negative", nameof(quantityBtc));
        }
        if (limitPriceEurPerBtc < 0)
        {
            throw new ArgumentException("LimitPriceEurPerBtc must be non-negative", nameof(limitPriceEurPerBtc));
        }
        VenueId = venueId;
        Side = side;
        QuantityBtc = quantityBtc;
        LimitPriceEurPerBtc = limitPriceEurPerBtc;
    }
}
