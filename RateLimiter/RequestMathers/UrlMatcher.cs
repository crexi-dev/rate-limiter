using RateLimiter.ClientStatistics;
using RateLimiter.Interfaces;

namespace RateLimiter.RequestMathers;

public class UrlMatcher : BaseMatcher
{
    public string Url { get; }

    public UrlMatcher(string url) : base(url)
    {
        Url = url;
    }

    public override bool IsMatch(RequestInformation request)
    {
        return IsAllRequestMatch || Url.Equals(request.Url);
    }

    public override IBucketIdentifier GetRequestId(RequestInformation request)
    {
        return new BucketIdentifier<string>(request.Url);
    }
}