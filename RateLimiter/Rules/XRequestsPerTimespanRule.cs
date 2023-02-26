using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rules
{
    public class XRequestsPerTimespanRule : IRule
    {
        private readonly int _maxRequests;
        private readonly TimeSpan _timeSpan;
        private readonly Dictionary<string, Queue<DateTime>> _requestTimes = new Dictionary<string, Queue<DateTime>>();

        public XRequestsPerTimespanRule(int maxRequests, TimeSpan timeSpan)
        {
            _maxRequests = maxRequests;
            _timeSpan = timeSpan;
        }

        public bool IsRequestAllowed(string resource, string client)
        {
            var key = $"{resource}:{client}";

            if (!_requestTimes.TryGetValue(key, out var requestTimes))
            {
                requestTimes = new Queue<DateTime>();
                _requestTimes[key] = requestTimes;
            }

            // Remove all request times that are outside the time span
            while (requestTimes.Count > 0 && DateTime.UtcNow - requestTimes.Peek() > _timeSpan)
            {
                requestTimes.Dequeue();
            }

            // Check if the number of requests is within the limit
            if (requestTimes.Count >= _maxRequests)
            {
                return false;
            }

            // Add the current request time and allow the request
            requestTimes.Enqueue(DateTime.UtcNow);
            return true;
        }
    }
}