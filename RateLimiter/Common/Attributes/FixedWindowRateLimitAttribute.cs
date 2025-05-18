namespace RateLimiter.Common.Attributes;

/// <summary>
/// Applies a fixed window rate limit.
/// </summary>
public class FixedWindowRateLimitAttribute : RateLimitAttribute
{
    public FixedWindowRateLimitAttribute(string name, int maxRequests, int timeWindowInSeconds)
        : base(name, maxRequests, timeWindowInSeconds)
    {
    }
}
