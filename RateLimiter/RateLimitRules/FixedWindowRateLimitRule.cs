using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimitRules
{
    public class FixedWindowRateLimitRule : RateLimitRuleBase
    {
        private readonly int _limit;
        private readonly TimeSpan _window;
        private readonly Dictionary<string, Dictionary<string, (DateTime, int)>> _requests;

        public FixedWindowRateLimitRule(int limit, TimeSpan window)
        {
            _limit = limit;
            _window = window;
            _requests = new Dictionary<string, Dictionary<string, (DateTime, int)>>();
        }

        public override bool IsLimitExceeded(string endpoint, string clientIdentifier)
        {
            if (!_requests.ContainsKey(endpoint) || !_requests[endpoint].ContainsKey(clientIdentifier))
            {
                return false;
            }

            var (startTime, count) = _requests[endpoint][clientIdentifier];
            if (DateTime.UtcNow - startTime > _window)
            {
                _requests[endpoint][clientIdentifier] = (DateTime.UtcNow, 0);
                return false;
            }

            return count >= _limit;
        }

        public override void RecordRequest(string endpoint, string clientIdentifier)
        {
            if (!_requests.ContainsKey(endpoint))
            {
                _requests[endpoint] = new Dictionary<string, (DateTime, int)>();
            }

            if (!_requests[endpoint].ContainsKey(clientIdentifier))
            {
                _requests[endpoint][clientIdentifier] = (DateTime.UtcNow, 0);
            }

            var (startTime, count) = _requests[endpoint][clientIdentifier];
            if (DateTime.UtcNow - startTime > _window)
            {
                startTime = DateTime.UtcNow;
                count = 0;
            }

            _requests[endpoint][clientIdentifier] = (startTime, count + 1);
        }
    }
}
