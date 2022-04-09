using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Implementation
{
    public class InMemCacheService : ICacheService
    {
        private static readonly Dictionary<string, object> _chache = new Dictionary<string, object>();

        public T Get<T>(string key)
        {
            _chache.TryGetValue(key, out object value);

            return (T)value;
        }

        public void Set<T>(string key, T value)
        {
            if (!_chache.ContainsKey(key))
            {
                _chache.Add(key, value);
            }

            _chache[key] = value;
        }
    }
}
