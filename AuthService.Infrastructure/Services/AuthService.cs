using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Models.Config;
using AuthService.Domain.Models.Requests;
using AuthService.Domain.Models.Responses;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _db;
        private readonly JwtSettings _jwt;

        public AuthService(AuthDbContext db, IOptions<JwtSettings> jwtOptions)
        {
            _db = db;
            _jwt = jwtOptions.Value;
        }

        public async Task<User> SeedAdminAsync(AdminSeedRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var normalizedAlias = request.Alias.Trim();

            // 1. Ensure Admin role exists
            var adminRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Admin" || r.Id == 1)
                ?? throw new Exception("Admin role not found.");

            // 2. Ensure admin doesn't already exist
            var adminExists = await _db.UserRoles.AnyAsync(ur => ur.RoleId == adminRole.Id);
            if (adminExists)
                throw new InvalidOperationException("An admin user already exists.");

            // 3. Check duplicates
            if (await _db.Users.AnyAsync(u => u.Email == normalizedEmail))
                throw new InvalidOperationException("Email already in use.");

            if (await _db.Users.AnyAsync(u => u.Alias == normalizedAlias))
                throw new InvalidOperationException("Alias already in use.");

            // 4. Hash password
            var passwordHash = PasswordHasher.HashPassword(request.Password);

            // 5. Create user
            var user = new User
            {
                Email = normalizedEmail,
                Alias = normalizedAlias,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var userRole = new UserRole
            {
                User = user,
                Role = adminRole
            };

            _db.Users.Add(user);
            _db.UserRoles.Add(userRole);

            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            var identifier = request.EmailOrAlias.Trim().ToLowerInvariant();

            // 1. Find user by email or alias
            var user = await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == identifier ||
                    u.Alias.ToLower() == identifier);

            if (user is null || !user.IsActive)
                throw new InvalidOperationException("Invalid credentials.");

            // 2. Verify password
            var validPassword = PasswordHasher.VerifyPassword(request.Password, user.PasswordHash);
            if (!validPassword)
                throw new InvalidOperationException("Invalid credentials.");

            // 3. Collect roles
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            // 4. Generate JWT
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_jwt.ExpiresMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("alias", user.Alias),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResult
            {
                UserId = user.Id,
                Email = user.Email,
                Alias = user.Alias,
                Roles = roles,
                AccessToken = tokenString,
                ExpiresAt = expires
            };
        }
    }
}
