using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;

namespace RateLimiter.Classes.Rules
{
    public class FixedNumberRule : IRateLimitRule
    {
        private readonly int _requestLimit;
        private readonly TimeSpan _timeSpan;

        public FixedNumberRule(int requestLimit, TimeSpan timeSpan)
        {
            _requestLimit = requestLimit;
            _timeSpan = timeSpan;
        }

        public bool IsRequestAllowed(string token, string resource)
        {
            var key = $"{resource}:{token}";
            if (!MemoryStore.Requests.ContainsKey(key))
            {
                MemoryStore.Requests[key] = new List<DateTime> { DateTime.Now };
                return true;
            }

            var requests = MemoryStore.Requests[key];
            requests.RemoveAll(dt => dt < DateTime.Now - _timeSpan);
            if (requests.Count < _requestLimit)
            {
                requests.Add(DateTime.Now);
                return true;
            }

            return false;
        }
    }
}
