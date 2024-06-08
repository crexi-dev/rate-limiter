using System;
using System.Collections.Generic;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules
{
    public class SlidingLimitRule : ILimitRule
    {
        private readonly TimeSpan _timeSpan;
        private readonly Dictionary<string, DateTime> _clientLastRequest = new();

        public SlidingLimitRule(TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
        }

        public bool IsRequestAllowed(string clientId, string ruleName)
        {
            var now = DateTime.UtcNow;

            if (_clientLastRequest.ContainsKey(clientId) && now - _clientLastRequest[clientId] <= _timeSpan)
            {
                return false;
            }
            _clientLastRequest[clientId] = now;

            return true;

        }
    }
}
