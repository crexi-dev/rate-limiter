namespace RateLimiter.Common.Attributes;

/// <summary>
/// Base attribute for rate limiting.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public abstract class RateLimitAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the rule.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the maximum number of requests allowed.
    /// </summary>
    public int MaxRequests { get; }

    /// <summary>
    /// Gets the time window in seconds.
    /// </summary>
    public int TimeWindowInSeconds { get; }

    protected RateLimitAttribute(string name, int maxRequests, int timeWindowInSeconds)
    {
        Name = name;
        MaxRequests = maxRequests;
        TimeWindowInSeconds = timeWindowInSeconds;
    }
}
