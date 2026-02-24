using MetaExchange.Domain.Orders;

namespace MetaExchange.Domain.BestExecution;


public sealed record BestExecutionPlan
{
    public OrderSide Side { get; }
    public decimal RequestedBtc { get; }
    public decimal FilledBtc { get; }
    public decimal TotalEur { get; }
    public IReadOnlyList<ChildOrder> Orders { get; }

    public BestExecutionPlan(OrderSide side, decimal requestedBtc, decimal filledBtc, decimal totalEur, IReadOnlyList<ChildOrder> orders)
    {
        if (requestedBtc < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(requestedBtc), "RequestedBtc must be non-negative");
        }
        if (filledBtc < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(filledBtc), "FilledBtc must be non-negative");
        }
        if (filledBtc > requestedBtc)
        {
            throw new ArgumentException("FilledBtc cannot exceed RequestedBtc", nameof(filledBtc));
        }
        if (totalEur < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(totalEur), "TotalEur must be non-negative");
        }

        Orders = orders ?? throw new ArgumentNullException(nameof(orders));
        if (Orders.Any(o => o is null))
        {
            throw new ArgumentException("Orders list must not contain null", nameof(orders));
        }

        Side = side;
        RequestedBtc = requestedBtc;
        FilledBtc = filledBtc;
        TotalEur = totalEur;
    }
}
