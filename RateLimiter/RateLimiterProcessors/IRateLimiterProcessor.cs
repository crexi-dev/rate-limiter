using RateLimiter.Models;
using RateLimiter.RateLimiterProcessors.Models;
using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimiterProcessors
{
    public interface IRateLimiterProcessor
    {
        ProcessorName Name { get; }

        RateLimiterStrategyResponse Process(IList<DateTime> requestTimes);
    }
}
