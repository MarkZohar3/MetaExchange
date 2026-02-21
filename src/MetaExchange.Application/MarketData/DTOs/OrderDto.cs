namespace MetaExchange.Application.MarketData.DTOs;

public sealed class OrderDto
{
    public Guid? Id { get; init; }
    public DateTime Time { get; init; }

    public required string Type { get; init; }
    public required string Kind { get; init; }

    public decimal Amount { get; init; }
    public decimal Price { get; init; }
}