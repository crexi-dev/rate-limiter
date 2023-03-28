using System.Collections.Generic;
using System.Linq;
using RateLimiter.ClientStatistics;

namespace RateLimiter.RequestMathers;

public class RegionMather : BaseMatcher
{
    public List<string>  Region { get; }

    public RegionMather(string regions) : base(regions)
    {
        Region = regions.Split(",").Select(c => c.ToLowerInvariant()).ToList();;
    }

    public override bool IsMatch(RequestInformation request)
    {
        return IsAllRequestMatch || Region.Contains(request.ClientId.Region.RegionCode);
    }

    public override IBucketIdentifier GetRequestId(RequestInformation request)
    {
        return new BucketIdentifier<string>(request.ClientId.Region.RegionCode);
    }
}