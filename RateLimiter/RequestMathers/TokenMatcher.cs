using RateLimiter.ClientStatistics;
using RateLimiter.Interfaces;

namespace RateLimiter.RequestMathers;

public class TokenMatcher : IMatcher
{
    public bool IsMatch(RequestInformation request)
    {
        return true;
    }

    public IBucketIdentifier GetRequestId(RequestInformation request)
    {
        return new BucketIdentifier<string>(request.ClientId.Token);
    }

    public IMatcher Combine(IMatcher mather)
    {
        return new CombineMatcher(this, mather);
    }
}