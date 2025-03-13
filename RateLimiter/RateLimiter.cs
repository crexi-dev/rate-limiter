using System.Collections.Generic;
using System.Linq;
using RateLimiter.Rules;

namespace RateLimiter;

public class RateLimiter
{
    private readonly List<IRateLimitRule> _rules;

    public RateLimiter(IEnumerable<IRateLimitRule> rules)
    {
        _rules = rules.ToList();
    }

    public bool AllowRequest(string clientId)
    {
        return _rules.All(rule => rule.AllowRequest(clientId));
    }
}
