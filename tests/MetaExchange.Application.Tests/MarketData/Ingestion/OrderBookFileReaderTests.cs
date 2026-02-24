using MetaExchange.Application.MarketData.Ingestion;

namespace MetaExchange.Application.Tests.MarketData.Ingestion;

public sealed class OrderBookFileReaderTests
{
    [Fact]
    public void ReadSnapshots_FromLines_InvalidLine_ThrowsFormatExceptionWithLineNumber()
    {
        // Arrange
        var lines = new[]
        {
            """50000.2	{"AcqTime":"2019-01-29T11:00:00Z","Bids":[],"Asks":[]}""",
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
