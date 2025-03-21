using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class ApiRateLimiter
    {
        private readonly List<RateLimitRule> _rules = new List<RateLimitRule>();

        public ApiRateLimiter(int maxRequests, int timeWindowSeconds)
        {
                AddRule(SampleRules.CreateTimeWindowRule(maxRequests, TimeSpan.FromSeconds(timeWindowSeconds)));
        }
        public ApiRateLimiter(TimeSpan hasPassed)
        {
            AddRule(SampleRules.CreateCertainTimespanPassed(hasPassed));
        }
        public ApiRateLimiter(int maxCallsinADay)
        {
            AddRule(SampleRules.CreateDailyQuotaRule(maxCallsinADay));
        }

        public void AddRule(RateLimitRule rule)
        {
            _rules.Add(rule);
        }
        public void RemoveAll()
        {
            _rules.Clear();
        }

        public bool IsAllowed(string clientId)
        {
            var timestamp = DateTime.UtcNow;
            return _rules.All(rule => rule(clientId, timestamp));
        }
    }
}