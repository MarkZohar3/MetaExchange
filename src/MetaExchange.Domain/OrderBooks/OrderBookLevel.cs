namespace MetaExchange.Domain.OrderBooks;


public sealed record PriceLevel
{
    public decimal Price { get; }
    public decimal Amount { get; }

    public PriceLevel(decimal price, decimal amount)
    {
        if (price <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Price must be positive");
        }
        if (amount <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");
        }

        Price = price;
        Amount = amount;
    }
}