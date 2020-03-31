using System;
using RateLimiter.Core.Rules;

namespace RateLimiter.API.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequestsPerTimespanAttribute : RateLimiterAttributeBase
    {
        public RequestsPerTimespanAttribute(string sourceIdentifier, int maxRequestCount, int seconds)
        {
            Rule = new RequestsPerTimespanRule(sourceIdentifier, maxRequestCount, seconds);
        }
    }
}
