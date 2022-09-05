using RateLimiter.Models;
using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimiterProcessors
{
    public interface IRateLimiterProcessor
    {
        ProcessorName Name { get; }

        RateLimiterProcessorResponse Process(IList<DateTime> requestTimes);
    }
}
