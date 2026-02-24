using MetaExchange.Domain.OrderBooks;

namespace MetaExchange.Domain.Venues;

public sealed record VenueSnapshot
{
    public string VenueId { get; }
    public OrderBookSnapshot Book { get; }
    public VenueBalances Balances { get; }

    public VenueSnapshot(string venueId, OrderBookSnapshot book, VenueBalances balances)
    {
        if (string.IsNullOrWhiteSpace(venueId))
        {
            throw new ArgumentException("venueId must be provided", nameof(venueId));
        }
        Book = book ?? throw new ArgumentNullException(nameof(book));
        Balances = balances ?? throw new ArgumentNullException(nameof(balances));
        VenueId = venueId;
    }
}