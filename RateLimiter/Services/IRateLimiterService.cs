using RateLimiter.Models;
using System;
using System.Collections.Generic;

namespace RateLimiter.Services
{
    public interface IRateLimiterService
    {
        IList<RateLimiterStrategyResponse> ProcessRequest(string clientId, IList<DateTime> requestTimes);
    }
}
