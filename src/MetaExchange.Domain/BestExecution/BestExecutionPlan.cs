using MetaExchange.Domain.Orders;

namespace MetaExchange.Domain.BestExecution;


public sealed record BestExecutionPlan
{
    public OrderSide Side { get; }
    public decimal Amount { get; }
    public decimal FilledBtc { get; }
    public decimal TotalEur { get; }
    public IReadOnlyList<ChildOrder> Orders { get; }

    public BestExecutionPlan(OrderSide side, decimal amount, decimal filledAmount, decimal totalEur, IReadOnlyList<ChildOrder> orders)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be non-negative");
        }
        if (filledAmount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(filledAmount), "FilledAmount must be non-negative");
        }
        if (filledAmount > amount)
        {
            throw new ArgumentException("FilledAmount cannot exceed Amount", nameof(filledAmount));
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
        Amount = amount;
        FilledBtc = filledAmount;
        TotalEur = totalEur;
    }
}
