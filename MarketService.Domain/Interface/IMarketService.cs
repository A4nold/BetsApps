using MarketService.Domain.Models;

namespace MarketService.Domain.Interfaces;

public interface IMarketService
{
    Task<MarketDto> CreateMarketAsync(CreateMarketCommand command, Guid creatorUserId, CancellationToken ct = default);
    Task<IReadOnlyList<MarketDto>> GetAllMarketsAsync(CancellationToken ct = default);
    Task<MarketDto?> GetMarketByIdAsync(Guid id, CancellationToken ct = default);
}
