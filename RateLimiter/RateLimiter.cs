using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter;

public class RateLimiter
{
    private readonly List<IRateLimitRule> _rules;
    private readonly IMemoryCache _cache;

    public RateLimiter(IMemoryCache cache)
    {
        _rules = new List<IRateLimitRule>();
        _cache = cache;
    }

    public void AddRule(IRateLimitRule rule)
    {
        _rules.Add(rule);
    }

    public async Task<bool> IsRequestAllowedAsync(string clientId, string region)
    {
        var cacheKey = $"{clientId}:{region}:result";
        if (_cache.TryGetValue(cacheKey, out bool cachedResult))
        {
            return cachedResult;
        }
        
        var tasks = _rules.Select(rule => rule.IsRequestAllowedAsync(clientId, region));
        var results = await Task.WhenAll(tasks);
        var result = results.All(r => r);

        _cache.Set(cacheKey, results, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _rules.Max(r => ((dynamic) r)._window ?? TimeSpan.FromMinutes(1))
        });

        return result;
    }
}