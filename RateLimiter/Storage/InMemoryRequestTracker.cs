using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RateLimiter.Models;

namespace RateLimiter.Storage
{
    public class InMemoryRequestTracker : IConcurrentRequestTracker
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _requestHistory =
            new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>();
        
        private readonly ConcurrentDictionary<string, DateTime> _lastRequestTime =
            new ConcurrentDictionary<string, DateTime>();
        
        private static string CreateKey(RequestContext context) =>
            $"{context.ClientToken}:{context.ResourcePath}";
        
        public Task<int> RecordRequestAndCountAsync(RequestContext context, TimeSpan timeWindow)
        {
            var key = CreateKey(context);
            var now = DateTime.UtcNow;
            var cutoff = now.Subtract(timeWindow);
            
            var history = _requestHistory.GetOrAdd(key, _ => new ConcurrentQueue<DateTime>());
            
            history.Enqueue(now);
            
            var count = 0;
            var oldRequests = new List<DateTime>();
            
            foreach (var timestamp in history)
            {
                if (timestamp >= cutoff)
                {
                    count++;
                }
                else
                {
                    oldRequests.Add(timestamp);
                }
            }
            
            foreach (var oldRequest in oldRequests)
            {
                history.TryDequeue(out _);
            }
            
            return Task.FromResult(count);
        }
        
        public Task<DateTime?> GetLastRequestTimeAsync(RequestContext context)
        {
            var key = CreateKey(context);
            
            if (_lastRequestTime.TryGetValue(key, out var timestamp))
            {
                return Task.FromResult<DateTime?>(timestamp);
            }
            
            return Task.FromResult<DateTime?>(null);
        }
        
        public Task RecordRequestTimeAsync(RequestContext context)
        {
            var key = CreateKey(context);
            _lastRequestTime[key] = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}