using System.Collections.Generic;

namespace RateLimiter;

public class RateLimitingManager
{
    private readonly Dictionary<string, RateLimiter> _resourceLimiters = new();

    public void ConfigureResourceRules(string resource, List<IRateLimitRule> rules)
    {
        _resourceLimiters[resource] = new RateLimiter(rules);
    }

    public bool IsRequestAllowed(RequestContext context)
    {
        return !_resourceLimiters.TryGetValue(context.Resource, out var limiter) || limiter.IsRequestAllowed(context);
    }
}