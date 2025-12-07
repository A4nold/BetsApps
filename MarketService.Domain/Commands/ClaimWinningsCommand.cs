namespace MarketService.Domain.Commands;

public sealed class ClaimWinningsCommand
{
    public Guid MarketId { get; init; }
    public Guid UserId { get; init; }

    // The bettor’s USDC token account on Solana
    public string BettorTokenAccount { get; init; } = default!;
}
