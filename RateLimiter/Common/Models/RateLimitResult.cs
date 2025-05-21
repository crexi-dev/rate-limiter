namespace RateLimiter.Common.Models;

/// <summary>
/// Represents the result of a rate limit evaluation.
/// </summary>
public class RateLimitResult
{
    /// <summary>
    /// Gets or sets whether the request is allowed.
    /// </summary>
    public bool IsAllowed { get; set; }

    /// <summary>
    /// Gets or sets the name of the rule that determined the result.
    /// </summary>
    public string Rule { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current counter value.
    /// </summary>
    public long Counter { get; set; }

    /// <summary>
    /// Gets or sets the rate limit.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Gets or sets the time window in seconds.
    /// </summary>
    public int TimeWindowInSeconds { get; set; }

    /// <summary>
    /// Gets or sets the time until the rate limit resets.
    /// </summary>
    public TimeSpan? ResetAfter { get; set; }

    /// <summary>
    /// Gets or sets an additional message.
    /// </summary>
    public string? Message { get; set; }
}
