using System.Security.Claims;
using MarketService.Domain.Interfaces;
using MarketService.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private readonly IPositionService _positionService;

    public PositionsController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PositionDto>>> GetMyPositions(CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var positions = await _positionService.GetPositionsForUserAsync(userId, ct);
        return Ok(positions);
    }
}
