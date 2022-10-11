using System;
using System.Collections.Generic;

namespace RateLimiter.Rules
{
    internal class MinPeriodRule : IRateLimiterRule
    {
        private readonly MaxRequestsPerPeriodRule _minPeriodRule;

        public MinPeriodRule(TimeSpan minPeriod)
        {
            _minPeriodRule = new MaxRequestsPerPeriodRule(1, minPeriod);
        }

        public bool IsAllowed(IReadOnlyList<DateTimeOffset> userRequests, DateTimeOffset requestDateTime)
        {
            return _minPeriodRule.IsAllowed(userRequests, requestDateTime);
        }
    }
}