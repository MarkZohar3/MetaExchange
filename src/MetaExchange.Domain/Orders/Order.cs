namespace MetaExchange.Domain.Orders;

public sealed record Order(
    Guid? Id,
    DateTime Time,
    OrderSide Side,
    OrderKind Kind,
    decimal Amount,
    decimal Price
);