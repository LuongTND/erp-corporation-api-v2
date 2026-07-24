using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;

namespace Infrastructure;

public static class GuidHelper
{
    public static Guid From(string input)
    {
        using var sha = SHA1.Create();
        var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        var bytes = new byte[16];
        Array.Copy(hash, bytes, 16);
        return new Guid(bytes);
    }
}

public static class TokenHelper
{
    public static string GenerateToken(int size = 64)
    {
        byte[] bytes = new byte[size];
        RandomNumberGenerator.Fill(bytes);
        return WebEncoders.Base64UrlEncode(bytes);
    }
}
