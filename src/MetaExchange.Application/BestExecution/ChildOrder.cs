using MetaExchange.Domain.Orders;

namespace MetaExchange.Application.BestExecution;

public sealed record ChildOrder(
    string VenueId,
    OrderSide Side,
    decimal QuantityBtc,
    decimal LimitPriceEurPerBtc);