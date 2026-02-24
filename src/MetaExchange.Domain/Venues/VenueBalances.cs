namespace MetaExchange.Domain.Venues;

public sealed record VenueBalances
{
    public decimal Eur { get; }
    public decimal Btc { get; }

    public VenueBalances(decimal eur, decimal btc)
    {
        if (eur < 0)
        {
            throw new ArgumentException("Eur must be non-negative", nameof(eur));
        }
        if (btc < 0)
        {
            throw new ArgumentException("Btc must be non-negative", nameof(btc));
        }
        Eur = eur;
        Btc = btc;
    }
}
