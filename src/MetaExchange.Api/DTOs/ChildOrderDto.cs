
namespace MetaExchange.Api.DTOs;

public sealed record ChildOrderDto(
    string Venue,
    string Side,
    decimal Price,
    decimal AmountBtc,
    decimal TotalEur);