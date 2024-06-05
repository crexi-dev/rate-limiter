using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class CooldownBetweenRequestsRule : RateLimitRule
    {
        private readonly TimeSpan _cooldown;
        private readonly ConcurrentDictionary<string, DateTime> _lastRequestTime = new();

        public CooldownBetweenRequestsRule(TimeSpan cooldown)
        {
            _cooldown = cooldown;
        }

        public override bool AllowRequest(string clientId)
        {
            var now = DateTime.UtcNow;
            if (_lastRequestTime.TryGetValue(clientId, out var lastRequest) && now - lastRequest < _cooldown)
            {
                return false;
            }

            _lastRequestTime[clientId] = now;
            return true;
        }
    }

}
