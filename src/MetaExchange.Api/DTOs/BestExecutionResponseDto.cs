namespace MetaExchange.Api.DTOs;

public sealed record BestExecutionResponseDto(
    string Side,
    decimal Amount,
    decimal FilledBtc,
    decimal TotalEur,
    IReadOnlyList<ChildOrderDto> Orders);
