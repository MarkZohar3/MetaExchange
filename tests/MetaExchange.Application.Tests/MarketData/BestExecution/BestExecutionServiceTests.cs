using MetaExchange.Application.BestExecution;
using MetaExchange.Domain.OrderBooks;
using MetaExchange.Domain.Orders;


namespace MetaExchange.Application.Tests.BestExecution;

public class BestExecutionServiceTests
{
    [Fact]
    public void Plan_AggregatesCorrectly()
    {
        // arrange: build two snapshots in memory and call the planner directly
        var book1 = new OrderBookSnapshot(
            UnixTimeSeconds: 0m,
            AcqTime: DateTime.UtcNow,
            Bids: new[] { new PriceLevel(100m, 1m) },
            Asks: Array.Empty<PriceLevel>());

        var book2 = new OrderBookSnapshot(
            UnixTimeSeconds: 0m,
            AcqTime: DateTime.UtcNow,
            Bids: new[] { new PriceLevel(110m, 1m) },
            Asks: Array.Empty<PriceLevel>());

        var venues = new[]
        {
            new VenueSnapshot("v1", book1, new VenueBalances(50_000m, 2m)),
            new VenueSnapshot("v2", book2, new VenueBalances(50_000m, 2m))
        };
        Assert.Equal(2, venues.Length); // sanity check

        IBestExecutionService service = new BestExecutionService();

        // act - we built bids above, so sell side will consume them
        var plan = service.Plan(OrderSide.Sell, 1.5m, venues);

        // assert
        Assert.Equal(OrderSide.Sell, plan.Side);
        Assert.Equal(1.5m, plan.RequestedBtc);
        Assert.Equal(1.5m, plan.FilledBtc);
        // proceeds from selling: 1*100 + 0.5*110 = 100 + 55 = 155
        Assert.Equal(160m, plan.TotalEur); // bids sorted high-to-low so v2 filled first
        Assert.Collection(plan.Orders,
            o =>
            {
                // highest bid (110) is filled first
                Assert.Equal("v2", o.VenueId);
                Assert.Equal(1m, o.QuantityBtc);
                Assert.Equal(110m, o.LimitPriceEurPerBtc);
            },
            o =>
            {
                Assert.Equal("v1", o.VenueId);
                Assert.Equal(0.5m, o.QuantityBtc);
                Assert.Equal(100m, o.LimitPriceEurPerBtc);
            });
    }
}
