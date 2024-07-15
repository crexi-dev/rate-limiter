using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimitRules
{
    public class SlidingWindowRateLimitRule : RateLimitRuleBase
    {
        private readonly int _limit;
        private readonly TimeSpan _window;
        private readonly Dictionary<string, Dictionary<string, Queue<DateTime>>> _requests;

        public SlidingWindowRateLimitRule(int limit, TimeSpan window)
        {
            _limit = limit;
            _window = window;
            _requests = new Dictionary<string, Dictionary<string, Queue<DateTime>>>();
        }

        public override bool IsLimitExceeded(string endpoint, string clientIdentifier)
        {
            if (!_requests.ContainsKey(endpoint) || !_requests[endpoint].ContainsKey(clientIdentifier))
            {
                return false;
            }

            var requests = _requests[endpoint][clientIdentifier];
            var now = DateTime.UtcNow;

            while (requests.Count > 0 && now - requests.Peek() > _window)
            {
                requests.Dequeue();
            }

            return requests.Count >= _limit;
        }

        public override void RecordRequest(string endpoint, string clientIdentifier)
        {
            if (!_requests.ContainsKey(endpoint))
            {
                _requests[endpoint] = new Dictionary<string, Queue<DateTime>>();
            }

            if (!_requests[endpoint].ContainsKey(clientIdentifier))
            {
                _requests[endpoint][clientIdentifier] = new Queue<DateTime>();
            }

            _requests[endpoint][clientIdentifier].Enqueue(DateTime.UtcNow);
        }
    }
}
