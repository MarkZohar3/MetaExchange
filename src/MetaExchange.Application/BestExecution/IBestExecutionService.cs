using MetaExchange.Domain.BestExecution;
using MetaExchange.Domain.Orders;
using MetaExchange.Domain.Venues;

namespace MetaExchange.Application.BestExecution;

public interface IBestExecutionService
{
    Task<BestExecutionPlan> PlanAsync(OrderSide side, decimal amount, IReadOnlyList<VenueSnapshot> venues);

    Task<BestExecutionPlan> PlanFromFileAsync(string venueFilePath, OrderSide side, decimal amount, CancellationToken cancellationToken = default);
}
