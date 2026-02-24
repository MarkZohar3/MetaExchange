using Microsoft.AspNetCore.Mvc;
using MetaExchange.Api.DTOs;
using MetaExchange.Api.Mapping;
using MetaExchange.Application.BestExecution;
using MetaExchange.Application;
using MetaExchange.Domain.Orders;
using Microsoft.AspNetCore.Http.HttpResults;
using MetaExchange.Domain.BestExecution;
using Microsoft.Extensions.Options;

namespace MetaExchange.Api.Controllers;

[ApiController]
[Route("best-execution")]
public sealed class BestExecutionController : ControllerBase
{
    private readonly IBestExecutionService _service;
    private readonly string _venuesDir;

    public BestExecutionController(IBestExecutionService service, IOptions<VenuesOptions> options)
    {
        _service = service;

        var venuesDir = options?.Value?.VenuesDirectory;
        _venuesDir = venuesDir
            ?? throw new InvalidOperationException("VenuesDirectory configuration is required.");
    }

    [HttpPost("plan")]
    [ProducesResponseType(typeof(BestExecutionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<BestExecutionResponseDto> Plan([FromBody] BestExecutionRequestDto req)
    {
        if (req.Amount <= 0m)
        {
            return BadRequest(new { error = "Amount must be > 0." });
        }

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
            plan = _service.PlanFromDirectory(_venuesDir, side, req.Amount);
        }
        catch (DirectoryNotFoundException)
        {
            return Problem(detail: $"Venue directory not found: {_venuesDir}", statusCode:500);
        }

        return Ok(plan.ToDto());
    }
}