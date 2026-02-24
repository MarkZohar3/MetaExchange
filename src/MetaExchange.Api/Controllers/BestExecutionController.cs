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
    private readonly string _venueFilePath;

    public BestExecutionController(IBestExecutionService service, IOptions<VenuesOptions> options)
    {
        _service = service;

        var venueFilePath = options?.Value?.VenueFilePath;
        _venueFilePath = venueFilePath
            ?? throw new InvalidOperationException("VenueFilePath configuration is required.");
    }

    [HttpPost("plan")]
    [ProducesResponseType(typeof(BestExecutionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BestExecutionResponseDto>> Plan([FromBody] BestExecutionRequestDto req)
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
            plan = await _service.PlanFromFileAsync(_venueFilePath, side, req.Amount, HttpContext.RequestAborted);
        }
        catch (FileNotFoundException)
        {
            return Problem(detail: $"Venue file not found: {_venueFilePath}", statusCode:500);
        }

        return Ok(plan.ToDto());
    }
}
