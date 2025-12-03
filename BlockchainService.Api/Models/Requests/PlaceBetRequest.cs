namespace BlockchainService.Api.Models.Requests;

public record PlaceBetRequest(
    string MarketPubkey,
    byte OutcomeIndex,
    ulong Amount,
    string UserPublicKey,
    string UserTokenAccount
);

