using Microsoft.Extensions.Caching.Memory;
using System;

namespace RateLimiter.Rules.TimeBasedRule;

public class TimeBasedRule : RateLimitRule<TimeBasedRuleOptions> 
{
    private readonly IMemoryCache _cache;

    public TimeBasedRule(
            TimeBasedRuleOptions options,
            IMemoryCache cache)
            : base(options)
    {
        _cache = cache;
    }

    public override bool IsRequestAllowed(AccessToken token, string route)
    {
        if (!ValidateCondition(token))
            return true;

        var key = $"{Options.Name}_{token.UserId}_{route}";
        var lastRequestTime = _cache.Get<DateTime?>(key);

        if (lastRequestTime.HasValue && (DateTime.UtcNow - lastRequestTime.Value) < Options.MinTimeBetweenRequests)
        {
            return false;
        }

        _cache.Set(key, DateTime.UtcNow, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = Options.MinTimeBetweenRequests });
        return true;
    }
}

