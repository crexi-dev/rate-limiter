using System.Linq;

namespace RateLimiter.Helper
{
    public static class KeyParser
    {
        public static string GetPrefix(string key)
        {
            return key.Split('-').FirstOrDefault() ?? string.Empty;
        }
    }
}