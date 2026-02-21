namespace MetaExchange.Application.MarketData.Parsing;

using MetaExchange.Domain.OrderBooks;

public sealed record ParsedSnapshot(
    decimal UnixTimeSeconds,
    OrderBookSnapshot Snapshot
);