namespace MarketService.Domain.Models;

public class CreateMarketCommand
{
    public string Question { get; set; } = default!;
    public DateTime EndTime { get; set; }
    public List<string> Outcomes { get; set; } = new();
}
