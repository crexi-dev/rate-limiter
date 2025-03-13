using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Models;

namespace RateLimiter.Services;

public class RateLimiterManager
{
    private readonly Dictionary<string, ResourceRateLimitConfig> _resourceLimits;

    public RateLimiterManager(IEnumerable<ResourceRateLimitConfig> resourceLimits)
    {
        _resourceLimits = resourceLimits.ToDictionary(r => r.Resource ?? string.Empty);
    }

    public RateLimitResult IsRequestAllowed(string clientId, string resource)
    {
        if (!_resourceLimits.TryGetValue(resource, out var config))
        {
            return new RateLimitResult { IsAllowed = false, RetryAfter = TimeSpan.Zero };
        }

        foreach (var rule in config.Rules)
        {
            var result = rule.IsRequestAllowed(clientId);
            if (!result.IsAllowed)
            {
                return result;
            }
        }

        return new RateLimitResult { IsAllowed = true, RetryAfter = TimeSpan.Zero };
    }
}
