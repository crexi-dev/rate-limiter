using System;

namespace RateLimiter.Models;

public sealed class RateLimitRule
{
    public RateLimitRule(TimeSpan period, long limit)
    {
        Period = period;

        if (limit < 1)
            throw new ArgumentOutOfRangeException($"{nameof(Limit)} must be positive.");

        Limit = limit;
    }

    public TimeSpan Period { get; }

    public long Limit { get; }
}
