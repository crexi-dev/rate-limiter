using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RateLimiter.Core.Models.Identity;

namespace RateLimiter.API.Resolvers;

public class IdentityResolver : IIdentityResolver
{
    public ClientRequestIdentity ResolveClient(HttpContext context)
    {
        string token = context.Request.Headers["token"];

        if (token is null)
        {
            throw new UnauthorizedAccessException("unauthorized");
        }

        try
        {
            string identityJson = Decrypt(token);
            ClientRequestIdentity identity = JsonSerializer.Deserialize<ClientRequestIdentity>(identityJson)!;
            
            return identity;

        }
        catch (Exception)
        {
            throw new UnauthorizedAccessException("unauthorized");
        }
    }
    
    public string Encrypt(string text)
    {
        byte[] b = Encoding.UTF8.GetBytes(text);
        byte[] encrypted = getAes().CreateEncryptor().TransformFinalBlock(b, 0, b.Length);
        return Convert.ToBase64String(encrypted);
    }

    public string Decrypt(string encrypted)
    {
        var b = Convert.FromBase64String(encrypted);
        var decrypted = getAes().CreateDecryptor().TransformFinalBlock(b, 0, b.Length);
        return Encoding.UTF8.GetString(decrypted);
    }

    private Aes getAes()
    {
        var keyBytes = new byte[16];
        var skeyBytes = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        Array.Copy(skeyBytes, keyBytes, Math.Min(keyBytes.Length, skeyBytes.Length));

        Aes aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.KeySize = 128;
        aes.Key = keyBytes;
        aes.IV = keyBytes;

        return aes;
    }
}