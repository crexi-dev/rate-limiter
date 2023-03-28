using RateLimiter.Interfaces;

namespace RateLimiter.RequestMathers;

public abstract class BaseMatcher : IMatcher
{
    protected bool IsAllRequestMatch;
    
    public BaseMatcher(string filter)
    {
        IsAllRequestMatch = MatcherHelper.IsAllRequestMatch(filter);
    }
    
    public abstract bool IsMatch(RequestInformation request);

    public abstract IBucketIdentifier GetRequestId(RequestInformation request);
    
    public IMatcher Combine(IMatcher mather)
    {
        return new CombineMatcher(this, mather);
    }
}