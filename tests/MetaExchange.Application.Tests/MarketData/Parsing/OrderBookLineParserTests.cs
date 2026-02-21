using MetaExchange.Application.MarketData.Parsing;
using Xunit;
using System.Globalization;

namespace MetaExchange.Application.Tests.MarketData.Parsing;

public sealed class OrderBookLineParserTests
{
    [Fact]
    public void TryParseLine_ValidLine_ParsesSnapshot()
    {
        // Arrange
        var line = """
            1548759600.25189	{"AcqTime":"2019-01-29T11:00:00.2518854Z",
            "Bids":[{"Order":{"Id":null,"Time":"0001-01-01T00:00:00","Type":"Buy","Kind":"Limit","Amount":0.01,"Price":2960.64}}],
            "Asks":[{"Order":{"Id":null,"Time":"0001-01-01T00:00:00","Type":"Sell","Kind":"Limit","Amount":0.405,"Price":2964.29}}]}
            """;

        // Act
        var isValid = OrderBookLineParser.TryParseLine(line, out var parsed, out var error);

        // Assert 
        Assert.True(isValid);
        Assert.Null(error);

        // Assert Metadata
        Assert.Equal(1548759600.25189m, parsed.UnixTimeSeconds);
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
        Assert.Equal(0.01m, bid.Quantity);

        var ask = parsed.Snapshot.Asks[0];
        Assert.Equal(2964.29m, ask.Price);
        Assert.Equal(0.405m, ask.Quantity);
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
        var line = """1548759600.25189 {"AcqTime":"2019-01-29T11:00:00Z"}""";

        var isValid = OrderBookLineParser.TryParseLine(line, out _, out var error);

        Assert.False(isValid );
        Assert.NotNull(error);
    }

    [Fact]
    public void TryParseLine_InvalidTimestamp_ReturnsFalse()
    {
        var line = """not-a-timestamp	{"AcqTime":"2019-01-29T11:00:00Z","Bids":[],"Asks":[]}""";

        var isValid = OrderBookLineParser.TryParseLine(line, out _, out var error);

        Assert.False(isValid);
        Assert.NotNull(error);
    }

    [Fact]
    public void TryParseLine_InvalidJson_ReturnsFalse()
    {
        var line = """1548759600.25189	{ this is not json }""";

        var isValid = OrderBookLineParser.TryParseLine(line, out _, out var error);

        Assert.False(isValid);
        Assert.NotNull(error);
    }
}