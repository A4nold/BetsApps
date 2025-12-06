namespace MarketService.Domain.Models;

public class PositionDto
{
    public Guid Id { get; set; }

    public Guid MarketId { get; set; }
    public string MarketPubkey { get; set; } = default!;
    public string Question { get; set; } = default!;

    public int OutcomeIndex { get; set; }
    public string OutcomeLabel { get; set; } = default!;

    public ulong StakeAmount { get; set; }
    public string TxSignature { get; set; } = default!;
    public DateTime PlacedAt { get; set; }
    public bool Claimed { get; set; }
    public DateTime? ClaimedAt { get; set; }
}

