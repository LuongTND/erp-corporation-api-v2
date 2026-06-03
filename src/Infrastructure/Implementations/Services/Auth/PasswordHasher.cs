using System.Security.Cryptography;

namespace Infrastructure.Implementations.Services.Auth;

public static class PasswordHasher
{
    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32; // 256 bit
    private const int Iterations = 10000;

    public static string Hash(string password)
    {
        using var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA256);
        var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
        var salt = Convert.ToBase64String(algorithm.Salt);
        return $"{Iterations}.{salt}.{key}";
    }

    public static bool Verify(string hash, string password)
    {
        var parts = hash.Split('.', 3);
        if (parts.Length != 3)
            return false;

        if (!int.TryParse(parts[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var key = parts[2];

        using var algorithm = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var keyToCheck = Convert.ToBase64String(algorithm.GetBytes(KeySize));
        return keyToCheck == key;
    }
}
