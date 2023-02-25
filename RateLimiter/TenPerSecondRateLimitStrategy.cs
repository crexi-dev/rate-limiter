using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public class PerTimespanRateLimitStrategy : IRateLimitStrategy
    {
        private readonly int _maxRequests;
        private readonly TimeSpan _interval;

        public PerTimespanRateLimitStrategy(int maxRequests, TimeSpan interval)
        {
            _maxRequests = maxRequests;
            _interval = interval;
        }

        public bool IsRequestAllowed(string username, string resource, IRequestRepository requestRepository)
        {
            var requestTimes = requestRepository.GetRequestsByTimeSpan(username, resource, _interval);
            if (requestTimes.Count >= _maxRequests)
            {
                return false;
            }
            return true;
        }
    }

}
