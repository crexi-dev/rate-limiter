namespace RateLimiter.Common.Attributes;

/// <summary>
/// Applies a region-based rate limit.
/// </summary>
public class RegionBasedRateLimitAttribute : RateLimitAttribute
{
    /// <summary>
    /// Gets the region this rule applies to.
    /// </summary>
    public string Region { get; }

    /// <summary>
    /// Gets the minimum time between requests in milliseconds (can be used for simple throttling).
    /// </summary>
    public int MinTimeBetweenRequestsMs { get; }

    public RegionBasedRateLimitAttribute(string name, string region, int maxRequests, int timeWindowInSeconds)
        : base(name, maxRequests, timeWindowInSeconds)
    {
        Region = region;
        MinTimeBetweenRequestsMs = 0;
    }
    
    public RegionBasedRateLimitAttribute(string name, string region, int maxRequests, int timeWindowInSeconds, int minTimeBetweenRequestsMs)
        : base(name, maxRequests, timeWindowInSeconds)
    {
        Region = region;
        MinTimeBetweenRequestsMs = minTimeBetweenRequestsMs;
    }
}