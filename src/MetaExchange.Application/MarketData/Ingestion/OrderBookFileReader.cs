namespace MetaExchange.Application.MarketData.Ingestion;

using MetaExchange.Application.MarketData.Parsing;

public static class OrderBookFileReader
{
    public static IEnumerable<ParsedSnapshot> ReadSnapshots(string filePath, int? maxLines = null)
    {
        return ReadSnapshots(File.ReadLines(filePath), maxLines);
    }
    public static IEnumerable<ParsedSnapshot> ReadSnapshots(IEnumerable<string> lines, int? maxLines = null)
    {
        var lineNo = 0;

        foreach (var line in lines)
        {
            lineNo++;

            if (maxLines is not null && lineNo > maxLines.Value)
                yield break;

            if (!OrderBookLineParser.TryParseLine(line, out var parsed, out var error))
                throw new FormatException($"Failed parsing line {lineNo}: {error}");

            yield return parsed;
        }
    }
}