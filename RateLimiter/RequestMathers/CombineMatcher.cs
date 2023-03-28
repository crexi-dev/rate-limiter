using System.Collections.Generic;
using System.Linq;
using RateLimiter.ClientStatistics;
using RateLimiter.Interfaces;

namespace RateLimiter.RequestMathers;

public class CombineMatcher : IMatcher
{
    private readonly List<IMatcher> _simpleMatcher;

    public CombineMatcher(params IMatcher[] simpleMatcher)
    {
        _simpleMatcher = simpleMatcher.ToList();
    }

    public bool IsMatch(RequestInformation request)
    {
        return _simpleMatcher.All(m => m.IsMatch(request));
    }

    public IBucketIdentifier GetRequestId(RequestInformation request)
    {
        var id = new CombineBucketIdentifier();
        foreach (var matcher in _simpleMatcher.Select(c => c.GetRequestId(request)))
        {
            id.Add(matcher);
        }
            
        return  id;
    }

    public IMatcher Combine(IMatcher mather)
    {
        _simpleMatcher.Add(mather);
        return this;
    }
}