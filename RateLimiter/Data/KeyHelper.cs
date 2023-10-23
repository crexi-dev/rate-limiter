using RateLimiter.Contracts;
using System;

namespace RateLimiter.Data
{
    internal class KeyHelper
    {
        internal static string GetKey(Guid token, ResourcesType type) => $"{token}_{type}";
    }
}
