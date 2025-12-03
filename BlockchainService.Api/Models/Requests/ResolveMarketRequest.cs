namespace BlockchainService.Api.Models.Requests;

public record ResolveMarketRequest(
    string MarketPubkey,
    byte WinningOutcomeIndex
);

