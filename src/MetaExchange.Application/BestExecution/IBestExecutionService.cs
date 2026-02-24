using MetaExchange.Domain.BestExecution;
using MetaExchange.Domain.Orders;
using MetaExchange.Domain.Venues;

namespace MetaExchange.Application.BestExecution;

public interface IBestExecutionService
{
    BestExecutionPlan Plan(OrderSide side, decimal requestedBtc, IReadOnlyList<VenueSnapshot> venues);

    BestExecutionPlan PlanFromDirectory(string venuesDir, OrderSide side, decimal requestedBtc);
}
