using MetaExchange.Domain.Venues;

namespace MetaExchange.Domain.OrderBooks;

public sealed record OrderBookSnapshot
{
    public VenueBalances VenueBalances { get; }
    public DateTime AcqTime { get; }

    public IReadOnlyList<PriceLevel> Bids { get; }
    public IReadOnlyList<PriceLevel> Asks { get; }

    public OrderBookSnapshot(VenueBalances venueBalances, DateTime acqTime, IReadOnlyList<PriceLevel> bids, IReadOnlyList<PriceLevel> asks)
    {
        ArgumentNullException.ThrowIfNull(venueBalances);

        if (acqTime < DateTime.UnixEpoch)
        {
            throw new ArgumentException("AcqTime must be non-negative", nameof(acqTime));
        }

        Bids = bids ?? throw new ArgumentNullException(nameof(bids));
        Asks = asks ?? throw new ArgumentNullException(nameof(asks));

        if (Bids.Any(b => b is null))
        {
            throw new ArgumentException("Bids list must not contain null", nameof(bids));
        }
        if (Asks.Any(a => a is null))
        {
            throw new ArgumentException("Asks list must not contain null", nameof(asks));
        }

        VenueBalances = venueBalances;
        AcqTime = acqTime;
    }
}