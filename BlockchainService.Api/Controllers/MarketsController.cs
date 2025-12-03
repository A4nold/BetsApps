using BlockchainService.Api.Models.Requests;
using BlockchainService.Api.Models.Responses;
using BlockchainService.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainService.Api.Controllers;

[ApiController]
[Route("api/markets")]
public class MarketsController : ControllerBase
{
    private readonly PredictionProgramClient _client;

    public MarketsController(PredictionProgramClient client)
    {
        _client = client;
    }

    [HttpPost("create")]
    public async Task<ActionResult<CreateMarketResponse>> Create([FromBody] CreateMarketRequest request)
    {
        var result = await _client.CreateMarketAsync(
            request.Question,
            request.Outcomes,
            request.EndTime,
            request.CollateralMint,
            request.VaultTokenAccount
        );

        var response = new CreateMarketResponse(
            result.MarketPubkey,
            result.TransactionSignature
        );

        return Ok(response);
    }
}
