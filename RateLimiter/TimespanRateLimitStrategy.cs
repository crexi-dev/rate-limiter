using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public class TimespanRateLimitStrategy : IRateLimitStrategy
    {
        private readonly int _maxRequests;
        private readonly TimeSpan _interval;

        public TimespanRateLimitStrategy(int maxRequests, TimeSpan interval)
        {
            _maxRequests = maxRequests;
            _interval = interval;
        }

        public bool IsRequestAllowed(string username, string resource, IRequestRepository requestRepository,string region)
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
