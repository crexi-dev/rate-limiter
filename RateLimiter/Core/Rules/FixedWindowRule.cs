using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Core.Rules
{
    public class FixedWindowRule : IRateLimitRule
    {
        private readonly int _maxRequests;
        private readonly TimeSpan _windowSize;
        private readonly ConcurrentDictionary<string, List<DateTime>> _requestTimestamps = new();

        public FixedWindowRule(int maxRequests, TimeSpan windowSize)
        {
            _maxRequests = maxRequests;
            _windowSize = windowSize;
        }

        public bool IsAllowed(string clientToken, string resourceId)
        {
            string key = $"{clientToken}:{resourceId}";
            var now = DateTime.UtcNow;

            // get or create timestamp list
            var timestamps = _requestTimestamps.GetOrAdd(key, _ =>
            {
                return new List<DateTime>();
            });

            // remove expired timestamps
            lock (timestamps)
            {
                timestamps.RemoveAll(time => now - time > _windowSize);

                // check if under limit
                if (timestamps.Count < _maxRequests)
                {
                    timestamps.Add(now);
                    return true;
                }

                return false;
            }
        }
    }
}