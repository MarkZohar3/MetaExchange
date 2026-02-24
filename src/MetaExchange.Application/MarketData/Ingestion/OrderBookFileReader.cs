namespace MetaExchange.Application.MarketData.Ingestion;

using MetaExchange.Application.MarketData.Parsing;

public static class OrderBookFileReader
{
    public static IEnumerable<ParsedSnapshot> ReadSnapshots(string filePath)
    {
        return ReadSnapshots(File.ReadLines(filePath));
    }
    public static IEnumerable<ParsedSnapshot> ReadSnapshots(IEnumerable<string> lines)
    {
        var lineNo = 0;

        foreach (var line in lines)
        {
            lineNo++;

            if (!OrderBookLineParser.TryParseLine(line, out var parsed, out var error))
            {
                throw new FormatException($"Failed parsing line {lineNo}: {error}");
            }

            yield return parsed;
        }
    }
}