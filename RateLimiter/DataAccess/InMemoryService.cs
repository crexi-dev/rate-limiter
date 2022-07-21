using RateLimiter.Model;
using System.Collections.Generic;

namespace RateLimiter.Cache
{
    public class InMemoryService : IStorageService
    {
        private Dictionary<string, UserRequestCache> _cache = new();

        public UserRequestCache GetToken(string key)
        {
            if (_cache.ContainsKey(key))
            {
                return _cache[key];
            }
            return null;
        }

        public void SetToken(string key, UserRequestCache token)
        {
            _cache[key] = token;
        }
    }
}
