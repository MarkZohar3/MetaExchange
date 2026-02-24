using MetaExchange.Domain.Orders;

namespace MetaExchange.Application.BestExecution;

public static class BestExecutionPlanner
{
    public static BestExecutionPlan Plan(
        OrderSide side,
        decimal requestedBtc,
        IReadOnlyList<VenueSnapshot> venues)
    {
        if (requestedBtc <= 0m || venues.Count == 0)
        {
            return new BestExecutionPlan(side, requestedBtc, 0m, 0m, Array.Empty<ChildOrder>());
        }

        // Build global candidate list (one candidate per price level per venue)
        var candidates = new List<Candidate>();

        foreach (var venue in venues)
        {
            var levels = side == OrderSide.Buy ? venue.Book.Asks : venue.Book.Bids;

            foreach (var level in levels)
            {
                if (level.Price <= 0m || level.Quantity <= 0m)
                {
                    continue;                
                }


                candidates.Add(new Candidate(
                    VenueId: venue.VenueId,
                    Price: level.Price,
                    Quantity: level.Quantity));
            }
        }

        // Sort by best price
        candidates.Sort((a, b) =>
            side == OrderSide.Buy
                ? a.Price.CompareTo(b.Price)   // buy: cheapest asks first
                : b.Price.CompareTo(a.Price)); // sell: highest bids first

        // Track balances per venue (mutable during planning)
        var eurByVenue = new Dictionary<string, decimal>(StringComparer.Ordinal);
        var btcByVenue = new Dictionary<string, decimal>(StringComparer.Ordinal);

        foreach (var v in venues)
        {
            eurByVenue[v.VenueId] = v.Balances.Eur;
            btcByVenue[v.VenueId] = v.Balances.Btc;
        }

        var remaining = requestedBtc;
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
                    VenueId: candidate.VenueId,
                    Side: OrderSide.Buy,
                    QuantityBtc: fillBtc,
                    LimitPriceEurPerBtc: price));

                var costEur = fillBtc * price;
                eurByVenue[candidate.VenueId] = eur - costEur;

                totalEur += costEur;
                remaining -= fillBtc;
            }
            else // Sell
            {
                var btc = btcByVenue[candidate.VenueId];

                var maxSellableBtc = btc;

                var fillBtc = Math.Min(candidate.Quantity, Math.Min(maxSellableBtc, remaining));
                if (fillBtc <= 0m)
                {
                    continue;                    
                }

                orders.Add(new ChildOrder(
                    VenueId: candidate.VenueId,
                    Side: OrderSide.Sell,
                    QuantityBtc: fillBtc,
                    LimitPriceEurPerBtc: price));

                btcByVenue[candidate.VenueId] = btc - fillBtc;

                var proceedsEur = fillBtc * price;
                totalEur += proceedsEur;
                remaining -= fillBtc;
            }
        }

        var filled = requestedBtc - remaining;

        return new BestExecutionPlan(
            Side: side,
            RequestedBtc: requestedBtc,
            FilledBtc: filled,
            TotalEur: totalEur,
            Orders: orders);
    }


    private sealed record Candidate(string VenueId, decimal Price, decimal Quantity);
}