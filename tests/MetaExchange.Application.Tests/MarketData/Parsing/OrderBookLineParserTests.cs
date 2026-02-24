using MetaExchange.Application.MarketData.Parsing;
using System.Globalization;

namespace MetaExchange.Application.Tests.MarketData.Parsing;

public sealed class OrderBookLineParserTests
{
    [Fact]
    public void TryParseLine_ValidLine_ParsesSnapshot()
    {
        // Arrange
        var line = """
            50000.2	{"AcqTime":"2019-01-29T11:00:00.2518854Z",
            "Bids":[{"Order":{"Id":null,"Time":"0001-01-01T00:00:00","Type":"Buy","Kind":"Limit","Amount":0.01,"Price":2960.64}}],
            "Asks":[{"Order":{"Id":null,"Time":"0001-01-01T00:00:00","Type":"Sell","Kind":"Limit","Amount":0.405,"Price":2964.29}}]}
            """;

        // Act
        var isValid = OrderBookLineParser.TryParseLine(line, out var parsed, out var error);

        // Assert 
        Assert.True(isValid);
        Assert.Null(error);

        // Assert Metadata
        Assert.Equal(50000m, parsed.Balances.Eur);
        Assert.Equal(2m, parsed.Balances.Btc);
        var expected = DateTimeOffset.Parse(
            "2019-01-29T11:00:00.2518854Z",
            CultureInfo.InvariantCulture);

        Assert.Equal(expected.UtcDateTime, parsed.Snapshot.AcqTime);

        // Assert Structure
        Assert.Single(parsed.Snapshot.Bids);
        Assert.Single(parsed.Snapshot.Asks);

        // Assert Values
        var bid = parsed.Snapshot.Bids[0];
        Assert.Equal(2960.64m, bid.Price);
        Assert.Equal(0.01m, bid.Amount);

        var ask = parsed.Snapshot.Asks[0];
        Assert.Equal(2964.29m, ask.Price);
        Assert.Equal(0.405m, ask.Amount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void TryParseLine_EmptyLine_ReturnsFalse(string line)
    {
        var isValid  = OrderBookLineParser.TryParseLine(line, out _, out var error);

        Assert.False(isValid );
        Assert.NotNull(error);
    }

    [Fact]
    public void TryParseLine_MissingTab_ReturnsFalse()
    {
        var line = """50000.2 {"AcqTime":"2019-01-29T11:00:00Z"}""";

        var isValid = OrderBookLineParser.TryParseLine(line, out _, out var error);

        Assert.False(isValid );
        Assert.NotNull(error);
    }

    [Fact]
    public void TryParseLine_InvalidBalances_ReturnsFalse()
    {
        var line = """not-balances	{"AcqTime":"2019-01-29T11:00:00Z","Bids":[],"Asks":[]}""";

        var isValid = OrderBookLineParser.TryParseLine(line, out _, out var error);

        Assert.False(isValid);
        Assert.NotNull(error);
    }

    [Fact]
    public void TryParseLine_InvalidJson_ReturnsFalse()
    {
        var line = """50000.2	{ this is not json }""";

        var isValid = OrderBookLineParser.TryParseLine(line, out _, out var error);

        Assert.False(isValid);
        Assert.NotNull(error);
    }
}
