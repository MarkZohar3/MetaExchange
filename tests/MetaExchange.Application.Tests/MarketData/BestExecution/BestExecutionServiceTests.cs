using MetaExchange.Application.BestExecution;
using MetaExchange.Domain.OrderBooks;
using MetaExchange.Domain.Orders;
using MetaExchange.Domain.Venues;

namespace MetaExchange.Application.Tests.BestExecution;

public class BestExecutionServiceTests
{
    [Fact]
    public async Task PlanFromFile_ReadsAllLinesAsVenueSnapshots()
    {
        // real service exercises parsing logic and the planner
        var service = new BestExecutionService();
        var venueFilePath = Path.GetTempFileName();

        try
        {
            var lines = new[]
            {
                "10000.1\t{\"AcqTime\":\"2026-01-01T00:00:00Z\",\"Bids\":[],\"Asks\":[{\"Order\":{\"Id\":null,\"Time\":\"0001-01-01T00:00:00\",\"Type\":\"Sell\",\"Kind\":\"Limit\",\"Amount\":1.0,\"Price\":100.0}}]}",
                "10000.1\t{\"AcqTime\":\"2026-01-01T00:00:01Z\",\"Bids\":[],\"Asks\":[{\"Order\":{\"Id\":null,\"Time\":\"0001-01-01T00:00:00\",\"Type\":\"Sell\",\"Kind\":\"Limit\",\"Amount\":1.0,\"Price\":90.0}}]}"
            };
            await File.WriteAllLinesAsync(venueFilePath, lines);


            var plan = await service.PlanFromFileAsync(venueFilePath, OrderSide.Buy, 1m);

            Assert.Equal(1m, plan.FilledAmount);
            Assert.Equal(90m, plan.TotalEur);
            Assert.Single(plan.Orders);
            Assert.Equal("Venue-2", plan.Orders[0].VenueId);
        }
        finally
        {
            File.Delete(venueFilePath);
        }
    }

    [Fact]
    public async Task Plan_Sell_AggregatesCorrectly()
    {
        // Arrange
        var book1 = new OrderBookSnapshot(
            venueBalances: new VenueBalances(1m, 1m),
            acqTime: DateTime.UtcNow,
            bids: new[] { new PriceLevel(100m, 1m) },
            asks: Array.Empty<PriceLevel>());

        var book2 = new OrderBookSnapshot(
            venueBalances: new VenueBalances(1m, 1m),
            acqTime: DateTime.UtcNow,
            bids: new[] { new PriceLevel(110m, 1m) },
            asks: Array.Empty<PriceLevel>());

        var venues = new[]
        {
            new VenueSnapshot("v1", book1, new VenueBalances(50_000m, 2m)),
            new VenueSnapshot("v2", book2, new VenueBalances(50_000m, 2m))
        };
        Assert.Equal(2, venues.Length); // sanity check

        var service = new BestExecutionService();

        // Act
        var plan = await service.PlanAsync(OrderSide.Sell, 1.5m, venues);

        // Assert
        Assert.Equal(OrderSide.Sell, plan.Side);
        Assert.Equal(1.5m, plan.Amount);
        Assert.Equal(1.5m, plan.FilledAmount);
        // proceeds from selling: 1*110 + 0.5*100 = 110 + 50 = 160
        Assert.Equal(160m, plan.TotalEur); // bids sorted high-to-low so v2 filled first
        Assert.Collection(plan.Orders,
            o =>
            {
                // highest bid (110) is filled first
                Assert.Equal("v2", o.VenueId);
                Assert.Equal(1m, o.Amount);
                Assert.Equal(110m, o.LimitPriceEurPerBtc);
            },
            o =>
            {
                Assert.Equal("v1", o.VenueId);
                Assert.Equal(0.5m, o.Amount);
                Assert.Equal(100m, o.LimitPriceEurPerBtc);
            });
    }

    [Fact]

    [Fact]
    public async Task Plan_BuyAggregatesCorrectly()
    {
        // Arrange: two venues offering asks at 100 and 110
        var book1 = new OrderBookSnapshot(
            venueBalances: new VenueBalances(1m, 1m),
            acqTime: DateTime.UtcNow,
            bids: Array.Empty<PriceLevel>(),
            asks: new[] { new PriceLevel(100m, 1m) });

        var book2 = new OrderBookSnapshot(
            venueBalances: new VenueBalances(1m, 1m),
            acqTime: DateTime.UtcNow,
            bids: Array.Empty<PriceLevel>(),
            asks: new[] { new PriceLevel(110m, 1m) });

        var venues = new[]
        {
            new VenueSnapshot("v1", book1, new VenueBalances(50_000m, 2m)),
            new VenueSnapshot("v2", book2, new VenueBalances(50_000m, 2m))
        };
        var service = new BestExecutionService();

        // Act
        var plan = await service.PlanAsync(OrderSide.Buy, 1.5m, venues);

        // Assert: cheapest ask filled first
        Assert.Equal(OrderSide.Buy, plan.Side);
        Assert.Equal(1.5m, plan.Amount);
        Assert.Equal(1.5m, plan.FilledAmount);
        // cost = 1*100 + 0.5*110 = 155
        Assert.Equal(155m, plan.TotalEur);
        Assert.Collection(plan.Orders,
            o =>
            {
                Assert.Equal("v1", o.VenueId);
                Assert.Equal(1m, o.Amount);
                Assert.Equal(100m, o.LimitPriceEurPerBtc);
            },
            o =>
            {
                Assert.Equal("v2", o.VenueId);
                Assert.Equal(0.5m, o.Amount);
                Assert.Equal(110m, o.LimitPriceEurPerBtc);
            });
    }

    [Fact]
    public async Task Plan_ThrowsWhenVenuesNull()
    {
        var service = new BestExecutionService();
        var venues = (IReadOnlyList<VenueSnapshot>)null!;
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await service.PlanAsync(OrderSide.Buy, 1m, venues));
    }

    [Fact]
    public async Task Plan_EmptyVenuesReturnsEmptyPlan()
    {
        var service = new BestExecutionService();
        var plan = await service.PlanAsync(OrderSide.Buy, 1m, Array.Empty<VenueSnapshot>());
        Assert.Equal(0m, plan.FilledAmount);
        Assert.Empty(plan.Orders);
    }

    [Fact]
    public async Task Plan_ThrowsWhenVenuesContainNull()
    {
        var service = new BestExecutionService();
        var list = new List<VenueSnapshot?> { null };
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.PlanAsync(OrderSide.Sell, 1m, list!));
    }
}
