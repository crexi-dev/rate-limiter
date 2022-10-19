using RateLimiter.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.DataAccess
{
    public class InMemoryCache: IStorage
    {
        private Dictionary<string, ClientRequestStorage> _cache = new();

        public ClientRequestStorage GetToken(string key)
        {
            if (_cache.ContainsKey(key))
            {
                return _cache[key];
            }
            
            return null;
        }

        public void SetToken(string key, ClientRequestStorage token)
        {
            _cache[key] = token;
        }
    }
}
