using System.Collections.Generic;
using MetaExchange.Domain.Orders;

namespace MetaExchange.Application.BestExecution;

public interface IBestExecutionService
{
    BestExecutionPlan Plan(OrderSide side, decimal requestedBtc, IReadOnlyList<VenueSnapshot> venues);

    BestExecutionPlan PlanFromDirectory(string venuesDir, OrderSide side, decimal requestedBtc);
}
