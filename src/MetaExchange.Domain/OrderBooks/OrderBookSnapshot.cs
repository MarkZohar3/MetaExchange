namespace MetaExchange.Domain.OrderBooks;

public sealed record OrderBookSnapshot
{
    public decimal UnixTimeSeconds { get; }
    public DateTime AcqTime { get; }

    public IReadOnlyList<PriceLevel> Bids { get; }
    public IReadOnlyList<PriceLevel> Asks { get; }

    public OrderBookSnapshot(decimal unixTimeSeconds, DateTime acqTime, IReadOnlyList<PriceLevel> bids, IReadOnlyList<PriceLevel> asks)
    {
        if (unixTimeSeconds < 0)
        {
            throw new ArgumentException("UnixTimeSeconds must be non-negative", nameof(unixTimeSeconds));
        }
        if (acqTime < DateTime.UnixEpoch)
        {
            throw new ArgumentException("AcqTime must be non-negative", nameof(acqTime));
        }

        UnixTimeSeconds = unixTimeSeconds;
        AcqTime = acqTime;
        Bids = bids;
        Asks = asks;
    }
}