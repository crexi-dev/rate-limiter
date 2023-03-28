using System.Collections.Generic;
using System.Linq;
using RateLimiter.Interfaces;

namespace RateLimiter;

public class RateLimitRuleStore
{
    public List<IRateLimitRule> _rules;

    public IReadOnlyCollection<IRateLimitRule> GetMatchedRules(RequestInformation request)
    {
        return _rules.Where(r => r.IsMatched(request)).ToList();
    }
}