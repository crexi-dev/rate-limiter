namespace RateLimiter.Common.Abstractions.Counters;

/// <summary>
/// Interface for rate limit counter storage.
/// </summary>
public interface IRateLimitCounter
{
    /// <summary>
    /// Gets the current count for a key.
    /// </summary>
    Task<long> GetCountAsync(string key);

    /// <summary>
    /// Sets the count for a key.
    /// </summary>
    Task SetCountAsync(string key, long count, TimeSpan expiry);

    /// <summary>
    /// Increments the count for a key.
    /// </summary>
    Task IncrementAsync(string key, long value, TimeSpan expiry);

    /// <summary>
    /// Decrements the count for a key.
    /// </summary>
    Task DecrementAsync(string key, long value);
    
    /// <summary>
    /// Resets counters for a specific client.
    /// </summary>
    Task ResetAsync(string clientId);
}
