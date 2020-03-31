using System;
using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Core;
using RateLimiter.Core.Rules;

namespace RateLimiter.API.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TimespanBetweenRequestsAttribute : RateLimiterAttributeBase
    {
        public TimespanBetweenRequestsAttribute(string sourceIdentifier, int ms)
        {
            Rule = new TimespanBetweenRequestsRule(sourceIdentifier, ms);
        }
    }
}
