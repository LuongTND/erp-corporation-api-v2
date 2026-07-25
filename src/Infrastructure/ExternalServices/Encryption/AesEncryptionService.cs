using System.Security.Cryptography;
using System.Text;

namespace Infrastructure;

[RegisterService(typeof(IEncryptionService), ServiceLifetime.Singleton)]
public sealed class AesEncryptionService(IAppConfiguration appConfiguration) : IEncryptionService
{
    private readonly byte[] _key = Convert.FromBase64String(appConfiguration.GetEncryptionOptions().Key);

    // Format stored in DB: base64(nonce[12] + ciphertext[n] + tag[16])
    public string Encrypt(string plaintext)
    {
        var nonce = RandomNumberGenerator.GetBytes(AesGcm.NonceByteSizes.MaxSize); // 12 bytes
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize]; // 16 bytes

        using var aes = new AesGcm(_key, AesGcm.TagByteSizes.MaxSize);
        aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        var result = new byte[nonce.Length + ciphertext.Length + tag.Length];
        nonce.CopyTo(result, 0);
        ciphertext.CopyTo(result, nonce.Length);
        tag.CopyTo(result, nonce.Length + ciphertext.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string ciphertext)
    {
        var data = Convert.FromBase64String(ciphertext);
        const int nonceSize = 12;
        const int tagSize = 16;

        var nonce = data[..nonceSize];
        var tag = data[^tagSize..];
        var cipher = data[nonceSize..^tagSize];
        var plaintext = new byte[cipher.Length];

        using var aes = new AesGcm(_key, tagSize);
        aes.Decrypt(nonce, cipher, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }
}
