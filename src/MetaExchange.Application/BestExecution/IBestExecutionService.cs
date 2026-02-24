using MetaExchange.Domain.BestExecution;
using MetaExchange.Domain.Orders;
using MetaExchange.Domain.Venues;

namespace MetaExchange.Application.BestExecution;

public interface IBestExecutionService
{
    BestExecutionPlan Plan(OrderSide side, decimal amount, IReadOnlyList<VenueSnapshot> venues);

    BestExecutionPlan PlanFromFile(string venueFilePath, OrderSide side, decimal amount);
}
