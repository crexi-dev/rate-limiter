using RateLimiter.Models;
using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimiterStrategies
{
    public interface IRateLimiterStrategy
    {
        RateLimiterStrategyResponse Process(List<DateTime> requestTimes);
    }
}
