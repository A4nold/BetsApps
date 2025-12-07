using MarketService.Domain.Entities;

namespace MarketService.Domain.Commands;

public sealed class ResolveMarketCommand
{
    public Guid MarketId { get; init; }
    public byte WinningOutcomeIndex { get; init; }
    public Guid ResolverUserId { get; init; }

    public ResolutionSource Source { get; init; }
    public string? EvidenceUrl { get; init; }
    public string? Notes { get; init; }
}

