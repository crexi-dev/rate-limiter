using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    internal class AccessRegistry
    {
        private ConcurrentDictionary<(string Resource, string Token), ConcurrentQueue<DateTime>> _accessAttempts = new();
        public ICollection<DateTime> GetAccessAttempts(string resource, string token)
        {
            if (_accessAttempts.TryGetValue((resource, token), out var accessAttempts))
            {
                return accessAttempts.ToList();
            }

            return Array.Empty<DateTime>();
        }

        public void AddAccessAttempt(string resource, string token, DateTime time)
        {
            var queue = _accessAttempts.GetOrAdd((resource, token), _ => new ConcurrentQueue<DateTime>());
            
            queue.Enqueue(time);
        }
    }
}