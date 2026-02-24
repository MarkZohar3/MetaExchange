namespace MetaExchange.Application.MarketData.Parsing;

using System.Globalization;
using System.Text.Json;
using MetaExchange.Application.MarketData.DTOs;
using MetaExchange.Domain.OrderBooks;
using MetaExchange.Domain.Venues;

public static class OrderBookLineParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static bool TryParseLine(string line, out ParsedSnapshot parsed, out string? error)
    {
        parsed = default!;
        error = null;

        if (string.IsNullOrWhiteSpace(line))
        {
            error = "Line is empty.";
            return false;
        }

        var tabIndex = line.IndexOf('\t');
        if (tabIndex <= 0 || tabIndex >= line.Length - 1)
        {
            error = "Line must be in format: <eurBalance>,<btcBalance><TAB><json>.";
            return false;
        }

        var balancesStr = line[..tabIndex].Trim();
        var jsonStr = line[(tabIndex + 1)..].Trim();

        var balanceParts = balancesStr.Split('.', 2, StringSplitOptions.TrimEntries);
        if (balanceParts.Length != 2
            || !decimal.TryParse(balanceParts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var eur)
            || !decimal.TryParse(balanceParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var btc))
        {
            error = $"Invalid balances prefix: '{balancesStr}'. Expected '<eurBalance>,<btcBalance>'.";
            return false;
        }

        VenueBalances balances;
        try
        {
            balances = new VenueBalances(eur, btc);
        }
        catch (Exception ex)
        {
            error = $"Invalid balances: {ex.Message}";
            return false;
        }

        OrderBookSnapshotDto? dto;
        try
        {
            dto = JsonSerializer.Deserialize<OrderBookSnapshotDto>(jsonStr, JsonOptions);
        }
        catch (JsonException je)
        {
            error = $"Invalid JSON: {je.Message}";
            return false;
        }

        if (dto is null)
        {
            error = "Deserialization produced null DTO.";
            return false;
        }

        OrderBookSnapshot snapshot;
        try
        {
            var venueBalances = new VenueBalances(balances.Eur, balances.Btc);
            snapshot = dto.ToDomain(venueBalances);
        }
        catch (Exception ex)
        {
            error = $"DTO mapping failed: {ex.Message}";
            return false;
        }

        parsed = new ParsedSnapshot(balances, snapshot);
        return true;
    }
}
