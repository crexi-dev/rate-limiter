namespace RateLimiter.Common.Models;

/// <summary>
/// Represents a rate limit configuration.
/// </summary>
public class RateLimit
{
    /// <summary>
    /// Gets or sets the maximum number of requests allowed in the time window.
    /// </summary>
    public int MaxRequests { get; set; }

    /// <summary>
    /// Gets or sets the time window in seconds.
    /// </summary>
    public int TimeWindowInSeconds { get; set; }
}
