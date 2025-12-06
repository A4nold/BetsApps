using System.Net.Http.Json;
using MarketService.Domain.Entities;
using MarketService.Domain.Interfaces;
using MarketService.Domain.Models;
using MarketService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MarketService.Infrastructure.Services;

public class MarketService : IMarketService
{
    private readonly MarketDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<MarketService> _logger;

    public MarketService(
        MarketDbContext db,
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<MarketService> logger)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    public async Task<MarketDto> CreateMarketAsync(CreateMarketCommand command, Guid creatorUserId, CancellationToken ct = default)
    {
        // 1. Call BlockchainService to create on-chain market
        var client = _httpClientFactory.CreateClient("BlockchainService");

        var blockchainRequest = new
        {
            question = command.Question,
            endTime = command.EndTime,
            outcomes = command.Outcomes
            // Add mint / vault later if needed
        };

        var response = await client.PostAsJsonAsync("/api/markets/create", blockchainRequest, ct);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("Failed to create on-chain market: {Status} {Body}", response.StatusCode, body);
            throw new InvalidOperationException("Failed to create on-chain market");
        }

        var chainResult = await response.Content.ReadFromJsonAsync<CreateMarketOnChainResponse>(cancellationToken: ct)
                          ?? throw new InvalidOperationException("Invalid response from BlockchainService");

        // 2. Save off-chain market + outcomes
        var market = new Market
        {
            Id = Guid.NewGuid(),
            MarketPubKey = chainResult.MarketPubkey,
            Question = command.Question,
            EndTime = command.EndTime,
            Status = MarketStatus.Open,
            CreatorUserId = creatorUserId,
            CreatedAt = DateTime.UtcNow
        };

        market.Outcomes = command.Outcomes
            .Select((label, index) => new MarketOutcome
            {
                MarketId = market.Id,
                OutcomeIndex = index,
                Label = label
            })
            .ToList();

        _db.Markets.Add(market);
        await _db.SaveChangesAsync(ct);

        return new MarketDto
        {
            Id = market.Id,
            MarketPubkey = market.MarketPubKey,
            Question = market.Question,
            EndTime = market.EndTime,
            Status = market.Status,
            Outcomes = market.Outcomes
                .OrderBy(o => o.OutcomeIndex)
                .Select(o => o.Label)
                .ToArray()
        };
    }

    public async Task<IReadOnlyList<MarketDto>> GetAllMarketsAsync(CancellationToken ct = default)
    {
        var markets = await _db.Markets
            .Include(m => m.Outcomes)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(ct);

        return markets.Select(m => new MarketDto
        {
            Id = m.Id,
            MarketPubkey = m.MarketPubKey,
            Question = m.Question,
            EndTime = m.EndTime,
            Status = m.Status,
            Outcomes = m.Outcomes
                .OrderBy(o => o.OutcomeIndex)
                .Select(o => o.Label)
                .ToArray()
        }).ToList();
    }

    public async Task<MarketDto?> GetMarketByIdAsync(Guid id, CancellationToken ct = default)
    {
        var m = await _db.Markets
            .Include(x => x.Outcomes)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        if (m == null) return null;

        return new MarketDto
        {
            Id = m.Id,
            MarketPubkey = m.MarketPubKey,
            Question = m.Question,
            EndTime = m.EndTime,
            Status = m.Status,
            Outcomes = m.Outcomes
                .OrderBy(o => o.OutcomeIndex)
                .Select(o => o.Label)
                .ToArray()
        };
    }

    private sealed class CreateMarketOnChainResponse
    {
        public string MarketPubkey { get; set; } = default!;
        public string TransactionSignature { get; set; } = default!;
    }
}
