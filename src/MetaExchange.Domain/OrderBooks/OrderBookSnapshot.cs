namespace MetaExchange.Domain.OrderBooks;

public sealed record OrderBookSnapshot(
    decimal UnixTimeSeconds,
    DateTime AcqTime,
    IReadOnlyList<PriceLevel> Bids,
    IReadOnlyList<PriceLevel> Asks
);