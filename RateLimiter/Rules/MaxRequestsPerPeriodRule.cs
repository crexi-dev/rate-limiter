using System;
using System.Collections.Generic;

namespace RateLimiter.Rules
{
    internal class MaxRequestsPerPeriodRule : IRateLimiterRule
    {
        private readonly int _maxRequestsPerPeriod;
        private readonly TimeSpan _period;

        public MaxRequestsPerPeriodRule(int maxRequestsPerPeriod, TimeSpan period)
        {
            _maxRequestsPerPeriod = maxRequestsPerPeriod;
            _period = period;
        }

        public bool IsAllowed(IReadOnlyList<DateTimeOffset> userRequests, DateTimeOffset requestDateTime)
        {
            if (userRequests.Count == 0 || userRequests.Count < _maxRequestsPerPeriod) return true;

            var startPeriodDateTime = userRequests[userRequests.Count - _maxRequestsPerPeriod];

            return (requestDateTime - startPeriodDateTime) >= _period;
        }
    }
}