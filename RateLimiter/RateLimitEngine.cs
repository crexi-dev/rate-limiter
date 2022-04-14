using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    internal static class RateLimitEngine
    {
        internal static void RequestsPerTimeSpan(List<Request> oldRequests, RateLimitRule rule)
        {
            // Get number of requests in the last timeframe
            var count = oldRequests.Count(x => x.Timestamp > DateTime.Now.AddSeconds(-rule.LimitInSeconds));
            // Verify count is less than limit
            if (count > rule.RequestCount)
            {
                throw new RateLimitException("Too many requests were made in the alotted time");
            }
        }

        internal static void TimeSpanSinceLastRequest(Request request, List<Request> oldRequests, RateLimitRule rule)
        {
            // Can only submit requests after a certain timeout period
            var previousRequestTime = oldRequests.Max(x => x.Timestamp);
            var timespan = (request.Timestamp - previousRequestTime).TotalMilliseconds;
            if (timespan < new TimeSpan(0, 0, rule.TimeSinceLastRequestInSeconds).TotalMilliseconds)
            {
                throw new RateLimitException("This request was made too soon");
            }
        }
    }
}
