using MarketService.Domain.Commands;
using MarketService.Domain.Entities;
using MarketService.Domain.Interfaces;
using MarketService.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MarketService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketsController : ControllerBase
{
    private readonly IMarketService _marketService;
    private readonly IPositionService _positionService;

    public MarketsController(IMarketService marketService, IPositionService positionService)
    {
        _marketService = marketService;
        _positionService = positionService;
    }

    //Helper
    private Guid GetUserId() 
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? User.FindFirstValue("sub");
        return Guid.Parse(sub);
    }

    public sealed class ClaimWinningsRequest
    {
        public string BettorTokenAccount { get; set; } = default!;
        public string VaultTokenAccount { get; set; } = default!;   
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MarketDto>>> GetMarkets(CancellationToken ct)
    {
        var markets = await _marketService.GetAllMarketsAsync(ct);
        return Ok(markets);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<MarketDto>> GetMarket(Guid id, CancellationToken ct)
    {
        var market = await _marketService.GetMarketByIdAsync(id, ct);
        if (market == null) return NotFound();
        return Ok(market);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MarketDto>> CreateMarket(
        [FromBody] CreateMarketCommand command,
        CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var market = await _marketService.CreateMarketAsync(command, userId, ct);
        return CreatedAtAction(nameof(GetMarket), new { id = market.Id }, market);
    }

    [HttpPost("{marketId:guid}/bet")]
    [Authorize]
    public async Task<ActionResult<PositionDto>> PlaceBet(
        Guid marketId, [FromBody] PlaceBetCommand command,
        CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier ?? User.FindFirstValue("sub"));

        if(!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

        var position = await _positionService.PlaceBetAsync(marketId, userId, command, ct);
        return Ok(position);
    }

    [HttpPost("{marketId:guid}/resolve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResolveMarket(Guid marketId, 
        [FromBody] byte winningOutcomeIndex, 
        CancellationToken ct)
    {
        var userId = GetUserId();

        var command = new ResolveMarketCommand
        {
            MarketId = marketId,
            WinningOutcomeIndex = winningOutcomeIndex,
            ResolverUserId = userId
        };

        var resolution = await _marketService.ResolveMarketAsync(command, ct);

        return CreatedAtAction(nameof(ResolveMarket), new { id = resolution.Id }, resolution);
    }

    [HttpPost("{marketId:guid}/claim")]
    [Authorize]
    public async Task<IActionResult> ClaimWinnings (Guid marketId, [FromBody] ClaimWinningsRequest request,
        CancellationToken ct)
    {
        var userId = GetUserId();

        var command = new ClaimWinningsCommand
        {
            MarketId = marketId,
            UserId = userId,
            BettorTokenAccount = request.BettorTokenAccount,
            VaultTokenAccount = request.VaultTokenAccount
        };

        await _marketService.ClaimWinningsAsync(command, ct);

        return NoContent();
    }
}
