using System;

namespace RateLimiter.Models;

/// <summary>
/// Stores the initial access time and the numbers of calls made from that point.
/// </summary>
public struct RateLimitCounter
{
    public RateLimitCounter(DateTime startedAt, DateTime? exceededAt, long totalRequests)
    {
        StartedAt = startedAt;
        ExceededAt = exceededAt;
        TotalRequests = totalRequests;
    }

    public DateTime StartedAt { get; }

    public DateTime? ExceededAt { get; }

    public long TotalRequests { get; set; }
}
