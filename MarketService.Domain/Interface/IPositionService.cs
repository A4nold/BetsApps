using MarketService.Domain.Models;

namespace MarketService.Domain.Interfaces;

public interface IPositionService
{
    Task<IReadOnlyList<PositionDto>> GetPositionsForUserAsync(Guid userId, CancellationToken ct = default);
    Task<PositionDto> PlaceBetAsync(Guid marketId, Guid userId, PlaceBetCommand command, CancellationToken ct = default);
}

