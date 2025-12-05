namespace AuthService.Domain.Models.Config;

public class JwtSettings
{
    public string Key { get; set; } = null!;       // symmetric secret
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpiresMinutes { get; set; } = 60;
}
