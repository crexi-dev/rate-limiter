using System;
using System.Collections.Generic;

namespace RateLimiter.Storage
{
    public static class DataStorage
    {
        public static Dictionary<string, List<DateTime>> Requests { get; set; }

        public static Dictionary<string, string> TokenOrigins { get; set; }

        public static string GetTokenOrigin(string token)
        {
            return TokenOrigins[token];
        }
    }
}
