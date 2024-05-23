using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules;

public class SlidingWindowRateLimitRule : IRateLimitRule
{
    private readonly TimeSpan _minInterval;
    private readonly ConcurrentDictionary<string, DateTime> _lastRequestTime;
    private readonly IMemoryCache _cache;
    
    public SlidingWindowRateLimitRule(TimeSpan minInterval, IMemoryCache cache)
    {
        _minInterval = minInterval;
        _lastRequestTime = new ConcurrentDictionary<string, DateTime>();
        _cache = cache;
    }
    
    public Task<bool> IsRequestAllowedAsync(string clientId, string region)
    {
        var cacheKey = $"{clientId}:{region}";

        if (_cache.TryGetValue(cacheKey, out bool cachedResult))
        {
            return Task.FromResult(cachedResult);
        }
        
        var now = DateTime.UtcNow;
        var key = $"{clientId}:{region}";

        var lastRequestTime = _lastRequestTime.GetOrAdd(key, DateTime.MinValue);

        var result = false;
        if (now - lastRequestTime >= _minInterval)
        {
            _lastRequestTime[key] = now;
            result = true;
        }

        _cache.Set(cacheKey, result, _minInterval);
        return Task.FromResult(result);
    }
}