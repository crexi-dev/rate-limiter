using System;
using RateLimiter.Core;
using RateLimiter.Core.Rules;

namespace RateLimiter.API.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class GeographicMuxAttribute : RateLimiterAttributeBase
    {
        public GeographicMuxAttribute(
            string sourceIdentifier,
            int timespanBetweenRequestsMs,
            int requestsPerTimespanMaxRequestCount,
            int requestsPerTimespanSeconds)
        {
            Rule = new GeographicMuxRule(
                sourceIdentifier,
                timespanBetweenRequestsMs, 
                requestsPerTimespanMaxRequestCount,
                requestsPerTimespanSeconds);
        }
    }
}
