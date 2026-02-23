using MetaExchange.Domain.Orders;

namespace MetaExchange.Application.BestExecution;

public sealed record BestExecutionPlan(
    OrderSide Side,
    decimal RequestedBtc,
    decimal FilledBtc,
    decimal TotalEur,
    IReadOnlyList<ChildOrder> Orders);