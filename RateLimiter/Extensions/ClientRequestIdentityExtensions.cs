using System;
using System.Security.Cryptography;
using System.Text;
using RateLimiter.Models;

namespace RateLimiter.Extensions;

internal static class ClientRequestIdentityExtensions
{
    internal static string GetStorageKey(this ClientRequestIdentity identity, TimeSpan period)
    {
        var key = $"{identity.ClientId}_{period}_{identity.HttpVerb}_{identity.Path}";
        var idBytes = Encoding.UTF8.GetBytes(key);

        byte[] hashBytes;
        using (var algorithm = SHA1.Create())
        {
            hashBytes = algorithm.ComputeHash(idBytes);
        }

        return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
    }
}
