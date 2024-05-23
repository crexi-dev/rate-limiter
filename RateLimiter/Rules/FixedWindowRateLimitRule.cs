using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules;

public class FixedWindowRateLimitRule : IRateLimitRule
{
    private readonly int _maxRequests;
    private readonly TimeSpan _window;
    private readonly IMemoryCache _cache;
    private readonly ILogger<FixedWindowRateLimitRule> _logger;
    private readonly ConcurrentDictionary<string, List<DateTime>> _requestTimes;
    
    public FixedWindowRateLimitRule(int maxRequests, TimeSpan window, IMemoryCache cache, ILogger<FixedWindowRateLimitRule> logger)
    {
        _maxRequests = maxRequests;
        _window = window;
        _requestTimes = new ConcurrentDictionary<string, List<DateTime>>();
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
        var requestedTimes = _requestTimes.GetOrAdd(clientId, new List<DateTime>());
        
        lock (requestedTimes)
        {
            requestedTimes.Add(now);
            requestedTimes.RemoveAll(t => t <= now - _window);

            var result = requestedTimes.Count <= _maxRequests;
            _cache.Set(cacheKey, result, _window);
            
            _logger.LogInformation("Request {RequestCount} for client {ClientId} in region {Region} at {Time}. Allowed: {Result}", requestedTimes.Count, clientId, region, now, result);
            return Task.FromResult(result);
        }
    }
}