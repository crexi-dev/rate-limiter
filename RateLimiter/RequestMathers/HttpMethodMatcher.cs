using System.Collections.Generic;
using System.Linq;
using RateLimiter.ClientStatistics;

namespace RateLimiter.RequestMathers;

public class HttpMethodMatcher : BaseMatcher
{
    public List<string> HttpMethods { get; }

    public HttpMethodMatcher(string httpMethods) : base(httpMethods)
    {
        HttpMethods = httpMethods.Split(",").Select(c => c.ToLowerInvariant()).ToList();
    }

    public override bool IsMatch(RequestInformation request)
    {
        return IsAllRequestMatch || HttpMethods.Contains(request.HttpMethod);
    }

    public override IBucketIdentifier GetRequestId(RequestInformation request)
    {
        return new BucketIdentifier<string>(request.HttpMethod);
    }
}