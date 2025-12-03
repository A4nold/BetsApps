namespace BlockchainService.Api.Models.Requests;

public record ClaimWinningsRequest(
    string MarketPubkey,
    string UserPublicKey,
    string UserTokenAccount
);

