using Microsoft.AspNetCore.Mvc;
using MetaExchange.Api.DTOs;
using MetaExchange.Api.Mapping;
using MetaExchange.Application.BestExecution;
using MetaExchange.Domain.Orders;
namespace MetaExchange.Api.Controllers;

[ApiController]
[Route("best-execution")]
public sealed class BestExecutionController : ControllerBase
{
    private readonly IBestExecutionService _service;
    private readonly string _venuesDir;

    public BestExecutionController(IBestExecutionService service, IConfiguration config)
    {
        _service = service;
        _venuesDir = config.GetValue<string>("VenuesDirectory")
            ?? throw new InvalidOperationException("VenuesDirectory configuration is required.");
    }

    [HttpPost("plan")]
    public ActionResult<BestExecutionResponseDto> Plan([FromBody] BestExecutionRequestDto req)
    {
        if (req.RequestedBtc <= 0m)
            return BadRequest(new { error = "RequestedBtc must be > 0." });

        OrderSide side;
        try
        {
            side = req.Side.ToOrderSide();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        BestExecutionPlan plan;
        try
        {
            plan = _service.PlanFromDirectory(_venuesDir, side, req.RequestedBtc);
        }
        catch (DirectoryNotFoundException)
        {
            return Problem(detail: $"Venue directory not found: {_venuesDir}", statusCode:500);
        }

        return Ok(plan.ToDto());
    }
}