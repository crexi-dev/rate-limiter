using System;
using RateLimiter.Library;

namespace RateLimiter.Library
{
    public interface IRateLimiterAlgorithm : IRequestsPerTimeSpanRateLimiter, ITimespanPassedSinceLastCallRateLimiter
    {
    }
}