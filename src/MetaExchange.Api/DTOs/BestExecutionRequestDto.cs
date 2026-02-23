namespace MetaExchange.Api.DTOs;

public sealed record BestExecutionRequestDto(
    string Side,
    decimal RequestedBtc);

