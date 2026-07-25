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
