using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules;

public class SlidingWindowRateLimitRule : IRateLimitRule
{
    private readonly TimeSpan _minInterval;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SlidingWindowRateLimitRule> _logger;
    private readonly ConcurrentDictionary<string, DateTime> _lastRequestTime;
    
    public SlidingWindowRateLimitRule(TimeSpan minInterval, IMemoryCache cache, ILogger<SlidingWindowRateLimitRule> logger)
    {
        _minInterval = minInterval;
        _lastRequestTime = new ConcurrentDictionary<string, DateTime>();
        _cache = cache;
        _logger = logger;
    }
    
    public Task<bool> IsRequestAllowedAsync(string clientId, string region)
    {
        var cacheKey = $"{clientId}:{region}";

        if (_cache.TryGetValue(cacheKey, out bool cachedResult))
        {
            _logger.LogInformation("Cache hit for client {ClientId} in region {Region}. Result: {Result}", clientId, region, cachedResult);
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
        _logger.LogInformation("Request for client {ClientId} in region {Region} at {Time}. Allowed: {Result}", clientId, region, now, result);
        return Task.FromResult(result);
    }
}