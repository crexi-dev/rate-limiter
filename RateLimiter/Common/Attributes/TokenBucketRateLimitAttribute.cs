namespace RateLimiter.Common.Attributes;

/// <summary>
/// Applies a token bucket rate limit.
/// </summary>
public class TokenBucketRateLimitAttribute : RateLimitAttribute
{
    /// <summary>
    /// Gets the token bucket capacity.
    /// </summary>
    public int BucketCapacity { get; }

    /// <summary>
    /// Gets the token refill rate per second.
    /// </summary>
    public double RefillRatePerSecond { get; }

    public TokenBucketRateLimitAttribute(string name, int bucketCapacity, double refillRatePerSecond, int timeWindowInSeconds)
        : base(name, bucketCapacity, timeWindowInSeconds)
    {
        BucketCapacity = bucketCapacity;
        RefillRatePerSecond = refillRatePerSecond;
    }
}
