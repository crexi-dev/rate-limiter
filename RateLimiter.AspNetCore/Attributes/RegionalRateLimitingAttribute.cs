namespace RateLimiter.AspNetCore.Attributes;

public class RegionalRateLimitingAttribute : RateLimitingAttribute
{
    public RegionalRateLimitingAttribute(string region, string interval, int limit = 1) : base(interval, limit)
    {
        Parameters = new List<RateLimiterParameter> { new(RateLimitingParameters.Region, region) };
    }
}