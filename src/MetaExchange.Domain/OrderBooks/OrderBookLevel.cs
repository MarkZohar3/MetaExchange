namespace MetaExchange.Domain.OrderBooks;

public sealed record PriceLevel(
    decimal Price,
    decimal Quantity
);