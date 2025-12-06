using MarketService.Domain.Entities;

namespace MarketService.Domain.Models;

public class MarketDto
{
    public Guid Id { get; set; }
    public string MarketPubkey { get; set; } = default!;
    public string Question { get; set; } = default!;
    public DateTime EndTime { get; set; }
    public MarketStatus Status { get; set; }
    public string[] Outcomes { get; set; } = Array.Empty<string>();
}
