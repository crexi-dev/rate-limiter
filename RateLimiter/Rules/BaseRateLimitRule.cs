using System.Collections.Generic;
using System.Linq;
using RateLimiter.Interfaces;

namespace RateLimiter.Rules;

public class BaseRateLimitRule : IRateLimitRule
{
    private IMatcher _matchers;

    public BaseRateLimitRule(IMatcher matchers)
    {
        _matchers = matchers;
    }

    public bool IsMatched(RequestInformation request)
    {
        return _matchers.IsMatch(request);
    }

    public IBucketIdentifier GetStatisticsId(RequestInformation request)
    {
        return _matchers.GetRequestId(request);
    }
}