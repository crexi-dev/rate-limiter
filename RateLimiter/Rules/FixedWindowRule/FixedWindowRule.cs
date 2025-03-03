using Microsoft.Extensions.Caching.Memory;

namespace RateLimiter.Rules.FixedWindowRule;

public class FixedWindowRule : RateLimitRule<FixedWindowRuleOptions>
{
    private readonly IMemoryCache _cache;

    public FixedWindowRule(
            FixedWindowRuleOptions options,
            IMemoryCache cache)
            : base(options)
    {
        _cache = cache;
    }

    public override bool IsRequestAllowed(AccessToken token, string route)
    {
        if (!ValidateCondition(token))
            return true;

        var cacheKey = $"{Options.Name}_{token.UserId}_{route}";
        var requestCount = _cache.Get<int?>(cacheKey) ?? 0;

        if (requestCount >= Options.Limit)
        {
            return false;
        }

        _cache.Set(cacheKey, requestCount + 1, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = Options.WindowSize
        });

        return true;
    }
}
