namespace RateLimiter.Common.Attributes;

/// <summary>
/// Applies a sliding window rate limit.
/// </summary>
public class SlidingWindowRateLimitAttribute : RateLimitAttribute
{
    public SlidingWindowRateLimitAttribute(string name, int maxRequests, int timeWindowInSeconds)
        : base(name, maxRequests, timeWindowInSeconds)
    {
    }
}
