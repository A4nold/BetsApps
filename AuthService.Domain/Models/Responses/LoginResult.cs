namespace AuthService.Domain.Models.Responses;

public class LoginResult
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Alias { get; set; } = null!;
    public IList<string> Roles { get; set; } = new List<string>();

    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
