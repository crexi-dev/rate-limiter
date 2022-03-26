using Microsoft.Extensions.Caching.Memory;
using System;

namespace RateLimiter.Rules
{
    public class SinceLastCallRule : IRule
    {
        public TimeSpan TimeSpan { get; }

        private readonly MemoryCache _cache;

        public SinceLastCallRule(TimeSpan timeSpan)
        {
            TimeSpan = timeSpan;

            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public bool IsAllowed(string token)
        {
            var now = DateTime.UtcNow;
            var result = true;

            if (_cache.TryGetValue<DateTime>(token, out var item))
            {
                result = now - item > TimeSpan;
            }

            _cache.Set(token, now);
            return result;
        }
    }
}
