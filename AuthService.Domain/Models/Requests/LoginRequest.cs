namespace AuthService.Domain.Models.Requests;

public class LoginRequest
{
    // Allow user to login with either email or alias
    public string EmailOrAlias { get; set; } = null!;
    public string Password { get; set; } = null!;
}
