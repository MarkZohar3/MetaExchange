using MetaExchange.Domain.OrderBooks;

namespace MetaExchange.Application.BestExecution;

public sealed record VenueSnapshot(
    string VenueId,
    OrderBookSnapshot Book,
    VenueBalances Balances);