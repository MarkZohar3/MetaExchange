namespace MetaExchange.Domain.OrderBooks;


public sealed record PriceLevel
{
    public decimal Price { get; }
    public decimal Quantity { get; }

    public PriceLevel(decimal price, decimal quantity)
    {
        if (price <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Price must be positive");
        }
        if (quantity <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive");
        }

        Price = price;
        Quantity = quantity;
    }
}