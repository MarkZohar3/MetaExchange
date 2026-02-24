using MetaExchange.Application.MarketData.Ingestion;

namespace MetaExchange.Application.Tests.MarketData.Ingestion;

public sealed class OrderBookFileReaderTests
{
    [Fact]
    public void ReadSnapshots_FromLines_MaxLines_StopsAfterMaxLines()
    {
        // Arrange
        var lines = new[]
        {
            """1548759600.1	{"AcqTime":"2019-01-29T11:00:00Z","Bids":[],"Asks":[]}""",
            """1548759601.2	{"AcqTime":"2019-01-29T11:00:01Z","Bids":[],"Asks":[]}""",
            """1548759602.3	{"AcqTime":"2019-01-29T11:00:02Z","Bids":[],"Asks":[]}""",
        };

        // Act
        var snapshots = OrderBookFileReader.ReadSnapshots(lines, maxLines: 2).ToList();

        // Assert
        Assert.Equal(2, snapshots.Count);
        Assert.Equal(1548759600.1m, snapshots[0].UnixTimeSeconds);
        Assert.Equal(1548759601.2m, snapshots[1].UnixTimeSeconds);
    }

    [Fact]
    public void ReadSnapshots_FromLines_InvalidLine_ThrowsFormatExceptionWithLineNumber()
    {
        // Arrange
        var lines = new[]
        {
            """1548759600.1	{"AcqTime":"2019-01-29T11:00:00Z","Bids":[],"Asks":[]}""",
            "BROKEN_LINE",
        };

        // Act
        var ex = Assert.Throws<FormatException>(() => OrderBookFileReader.ReadSnapshots(lines).ToList());

        // Assert
        Assert.Contains("line 2", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ReadSnapshots_FromLines_EmptyEnumerable_ReturnsEmpty()
    {
        // Arrange
        var lines = Array.Empty<string>();

        // Act
        var snapshots = OrderBookFileReader.ReadSnapshots(lines).ToList();

        // Assert
        Assert.Empty(snapshots);
    }
}