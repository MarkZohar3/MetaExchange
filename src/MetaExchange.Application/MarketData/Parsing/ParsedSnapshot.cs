namespace MetaExchange.Application.MarketData.Parsing;

using MetaExchange.Domain.Venues;
using MetaExchange.Domain.OrderBooks;

public sealed record ParsedSnapshot(
    VenueBalances Balances,
    OrderBookSnapshot Snapshot
);
