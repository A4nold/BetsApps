using System.Security.Cryptography;
using System.Text;

namespace AuthService.Infrastructure.Security;

public static class PasswordHasher
{
    // Adjust if you want stronger hashing (will be slower)
    private const int Iterations = 100_000;
    private const int SaltSize = 16;   // 128-bit
    private const int KeySize = 32;    // 256-bit

    public static string HashPassword(string password)
    {
        // Generate a random salt
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        // Derive the key
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256
        );

        var key = pbkdf2.GetBytes(KeySize);

        // Store as: {iterations}.{salt}.{key} (all base64)
        var result = $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        return result;
    }

    public static bool VerifyPassword(string password, string hash)
    {
        try
        {
            var parts = hash.Split('.');
            if (parts.Length != 3) return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256
            );

            var keyToCheck = pbkdf2.GetBytes(KeySize);

            return CryptographicOperations.FixedTimeEquals(keyToCheck, key);
        }
        catch
        {
            return false;
        }
    }
}
