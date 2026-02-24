using MetaExchange.Domain.Orders;
using MetaExchange.Domain.Venues;

namespace MetaExchange.Domain.BestExecution;

public static class BestExecutionPlanner
{
    public static BestExecutionPlan Plan(
        OrderSide side,
        decimal amount,
        IReadOnlyList<VenueSnapshot> venues)
    {
        ArgumentNullException.ThrowIfNull(venues);
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "amount must be non-negative");
        }
        if (venues.Count == 0 || amount == 0m)
        {
            return new BestExecutionPlan(side, amount, 0m, 0m, Array.Empty<ChildOrder>());
        }
        if (venues.Any(v => v is null))
        {
            throw new ArgumentException("venues list must not contain null", nameof(venues));
        }

        // Build global candidate list (one candidate per price level per venue)
        var candidates = new List<Candidate>();

        foreach (var venue in venues)
        {
            var levels = side == OrderSide.Buy ? venue.Book.Asks : venue.Book.Bids;

            foreach (var level in levels)
            {
                if (level.Price <= 0m || level.Amount <= 0m)
                {
                    continue;                
                }

                candidates.Add(new Candidate(
                    VenueId: venue.VenueId,
                    Price: level.Price,
                    Quantity: level.Amount));
            }
        }

        candidates.Sort((a, b) =>
            side == OrderSide.Buy
                ? a.Price.CompareTo(b.Price)   // buy: cheapest asks first
                : b.Price.CompareTo(a.Price)); // sell: highest bids first

        var eurByVenue = new Dictionary<string, decimal>(StringComparer.Ordinal);
        var btcByVenue = new Dictionary<string, decimal>(StringComparer.Ordinal);

        foreach (var v in venues)
        {
            eurByVenue[v.VenueId] = v.Balances.Eur;
            btcByVenue[v.VenueId] = v.Balances.Btc;
        }

        var remaining = amount;
        var totalEur = 0m;
        var orders = new List<ChildOrder>();

        foreach (var candidate in candidates)
        {
            if (remaining <= 0m)
            {
                break;
            }


            var price = candidate.Price;

            if (side == OrderSide.Buy)
            {
                var eur = eurByVenue[candidate.VenueId];

                var maxAffordableBtc = eur / price;

                var fillBtc = Math.Min(candidate.Quantity, Math.Min(maxAffordableBtc, remaining));
                if (fillBtc <= 0m)
                {
                    continue;  
                }


                orders.Add(new ChildOrder(
                    venueId: candidate.VenueId,
                    side: OrderSide.Buy,
                    amount: fillBtc,
                    limitPriceEurPerBtc: price));

                var costEur = fillBtc * price;
                eurByVenue[candidate.VenueId] = eur - costEur;

                totalEur += costEur;
                remaining -= fillBtc;
            }
            else // Sell
            {
                var btc = btcByVenue[candidate.VenueId];

                var maxSellableBtc = btc;

                var fillAmount = Math.Min(candidate.Quantity, Math.Min(maxSellableBtc, remaining));
                if (fillAmount <= 0m)
                {
                    continue;                    
                }

                orders.Add(new ChildOrder(
                    venueId: candidate.VenueId,
                    side: OrderSide.Sell,
                    amount: fillAmount,
                    limitPriceEurPerBtc: price));

                btcByVenue[candidate.VenueId] = btc - fillAmount;

                var proceedsEur = fillAmount * price;
                totalEur += proceedsEur;
                remaining -= fillAmount;
            }
        }

        var filled = amount - remaining;

        return new BestExecutionPlan(
            side: side,
            amount: amount,
            filledAmount: filled,
            totalEur: totalEur,
            orders: orders);
    }


    private sealed record Candidate(string VenueId, decimal Price, decimal Quantity);
}