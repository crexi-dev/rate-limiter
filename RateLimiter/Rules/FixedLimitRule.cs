using System;
using System.Collections.Generic;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules
{
    public class FixedLimitRule : ILimitRule
    {
        private readonly int _limit;
        private readonly TimeSpan _timeSpan;
        private readonly Dictionary<string, List<DateTime>> _clientRequests = new();

        public FixedLimitRule(int limit, TimeSpan timeSpan)
        {
            _limit = limit;
            _timeSpan = timeSpan;
        }

        public bool IsRequestAllowed(string clientId, string ruleName)
        {
            if (!_clientRequests.ContainsKey(clientId))
            {
                _clientRequests[clientId] = new List<DateTime>();
            }

            var now = DateTime.UtcNow;
            _clientRequests[clientId].RemoveAll(entry => now - entry > _timeSpan);

            if (_clientRequests[clientId].Count >= _limit)
            {
                return false;
            }

            _clientRequests[clientId].Add(now);

            return true;
        }
    }
}
