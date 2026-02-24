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
        if (requestedBtc < 0)
        {
            throw new ArgumentException("RequestedBtc must be non-negative", nameof(requestedBtc));
        }
        if (filledBtc < 0)
        {
            throw new ArgumentException("FilledBtc must be non-negative", nameof(filledBtc));
        }
        if (totalEur < 0)
        {
            throw new ArgumentException("TotalEur must be non-negative", nameof(totalEur));
        }

        Side = side;
        RequestedBtc = requestedBtc;
        FilledBtc = filledBtc;
        TotalEur = totalEur;
        Orders = orders;
    }
}
