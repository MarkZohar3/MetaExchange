namespace MetaExchange.Application.MarketData.Parsing;

using System.Globalization;
using System.Text.Json;
using MetaExchange.Application.MarketData.DTOs;
using MetaExchange.Domain.OrderBooks;

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
            error = "Line must be in format: <unixSeconds><TAB><json>.";
            return false;
        }

        var unixStr = line[..tabIndex].Trim();
        var jsonStr = line[(tabIndex + 1)..].Trim();

        if (!decimal.TryParse(unixStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var unixSeconds))
        {
            error = $"Invalid unix timestamp: '{unixStr}'.";
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
            snapshot = dto.ToDomain(unixSeconds);
        }
        catch (Exception ex)
        {
            error = $"DTO mapping failed: {ex.Message}";
            return false;
        }

        parsed = new ParsedSnapshot(unixSeconds, snapshot);
        return true;
    }
}