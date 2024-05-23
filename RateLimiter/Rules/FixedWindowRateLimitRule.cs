using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules;

public class FixedWindowRateLimitRule : IRateLimitRule
{
    private readonly int _maxRequests;
    private readonly TimeSpan _window;
    private readonly ConcurrentDictionary<string, List<DateTime>> _requestTimes;
    private readonly IMemoryCache _cache;

    public FixedWindowRateLimitRule(int maxRequests, TimeSpan window, IMemoryCache cache)
    {
        _maxRequests = maxRequests;
        _window = window;
        _requestTimes = new ConcurrentDictionary<string, List<DateTime>>();
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
        var requestedTimes = _requestTimes.GetOrAdd(clientId, new List<DateTime>());
        
        lock (requestedTimes)
        {
            requestedTimes.Add(now);
            requestedTimes.RemoveAll(t => t <= now - _window);

            var result = requestedTimes.Count <= _maxRequests;
            _cache.Set(cacheKey, result, _window);
            
            return Task.FromResult(result);
        }
    }
}