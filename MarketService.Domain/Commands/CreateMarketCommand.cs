namespace MarketService.Domain.Commands;

public class CreateMarketCommand
{
    public string Question { get; set; } = default!;
    public DateTime EndTime { get; set; }
    public List<string> Outcomes { get; set; } = new();
    public string CollateralMint { get; set; } = default!;
    public string VaultTokenAccount { get; set; } = default!;
}
