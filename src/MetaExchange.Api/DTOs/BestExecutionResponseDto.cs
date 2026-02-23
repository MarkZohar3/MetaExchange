namespace MetaExchange.Api.DTOs;

public sealed record BestExecutionResponseDto(
    string Side,
    decimal RequestedBtc,
    decimal FilledBtc,
    decimal TotalEur,
    IReadOnlyList<ChildOrderDto> Orders);
