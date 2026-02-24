using MetaExchange.Api.DTOs;
using MetaExchange.Domain.BestExecution;
using MetaExchange.Domain.Orders;

namespace MetaExchange.Api.Mapping;

public static class BestExecutionMapper
{
    // outgoing
    public static BestExecutionResponseDto ToDto(this BestExecutionPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        return new BestExecutionResponseDto(
            Side: plan.Side.ToString(),
            Amount: plan.Amount,
            FilledBtc: plan.FilledBtc,
            TotalEur: plan.TotalEur,
            Orders: plan.Orders.Select(ToDto).ToArray());
    }

    public static ChildOrderDto ToDto(this ChildOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var totalEur = order.QuantityBtc * order.LimitPriceEurPerBtc;

        return new ChildOrderDto(
            Venue: order.VenueId,
            Side: order.Side.ToString(),
            Price: order.LimitPriceEurPerBtc,
            AmountBtc: order.QuantityBtc,
            TotalEur: totalEur);
    }

    // incoming
    public static OrderSide ToOrderSide(this string text)
    {
        if (Enum.TryParse<OrderSide>(text, ignoreCase: true, out var side))
        {
            return side;
        }
        throw new ArgumentException("Side must be 'Buy' or 'Sell'.", nameof(text));
    }
}
