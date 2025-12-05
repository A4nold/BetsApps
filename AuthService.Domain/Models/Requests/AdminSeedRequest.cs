namespace AuthService.Domain.Models.Requests;

public class AdminSeedRequest
{
    public string Email { get; set; } = null!;
    public string Alias { get; set; } = null!;
    public string Password { get; set; } = null!;
}

