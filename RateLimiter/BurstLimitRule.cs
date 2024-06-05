using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class BurstLimitRule : RateLimitRule
    {
        private readonly int _burstLimit;
        private readonly TimeSpan _burstPeriod;
        private readonly TimeSpan _cooldown;
        private readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();
        private readonly ConcurrentDictionary<string, DateTime> _cooldowns = new();

        public BurstLimitRule(int burstLimit, TimeSpan burstPeriod, TimeSpan cooldown)
        {
            _burstLimit = burstLimit;
            _burstPeriod = burstPeriod;
            _cooldown = cooldown;
        }

        public override bool AllowRequest(string clientId)
        {
            var now = DateTime.UtcNow;

            if (_cooldowns.TryGetValue(clientId, out var cooldownEnd) && now < cooldownEnd)
            {
                return false;
            }

            _requests.AddOrUpdate(clientId, new List<DateTime> { now }, (key, list) =>
            {
                list.Add(now);
                list.RemoveAll(t => t < now - _burstPeriod);
                return list;
            });

            if (_requests[clientId].Count > _burstLimit)
            {
                _cooldowns[clientId] = now + _cooldown;
                return false;
            }

            return true;
        }
    }
}
