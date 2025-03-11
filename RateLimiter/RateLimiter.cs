using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter;

public class RateLimiter
{
    private readonly List<IRateLimitRule> _rules;

    public RateLimiter(List<IRateLimitRule> rules)
    {
        _rules = rules ?? [];
    }
    public bool IsRequestAllowed(RequestContext context)
    {
        var now = DateTime.UtcNow;
        return _rules.All(rule => rule.IsRequestAllowed(context, now));
    }
}