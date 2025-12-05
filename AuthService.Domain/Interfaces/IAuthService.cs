using AuthService.Domain.Entities;
using AuthService.Domain.Models.Requests;
using AuthService.Domain.Models.Responses;

namespace AuthService.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<User> SeedAdminAsync(AdminSeedRequest request);
        Task<LoginResult> LoginAsync(LoginRequest request);
    }
}
