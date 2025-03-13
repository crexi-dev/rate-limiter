using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Models;
using RateLimiter.Rules;

namespace RateLimiter.Rules;

public class SlidingWindowRule : IRateLimitRule
{
    private readonly int _maxRequests;
    private readonly TimeSpan _windowSize;
    private readonly Dictionary<string, List<DateTime>> _clientRequests = new();

    public SlidingWindowRule(int maxRequests, TimeSpan windowSize)
    {
        _maxRequests = maxRequests;
        _windowSize = windowSize;
    }

    public RateLimitResult IsRequestAllowed(string clientId)
    {
        lock (_clientRequests)
        {
            DateTime now = DateTime.UtcNow;
            _clientRequests.TryGetValue(clientId, out var timestamps);
            
            timestamps ??= new List<DateTime>();
            timestamps.RemoveAll(t => now - t > _windowSize);

            if (timestamps.Count < _maxRequests)
            {
                timestamps.Add(now);
                _clientRequests[clientId] = timestamps;
                return new RateLimitResult { IsAllowed = true, RetryAfter = TimeSpan.Zero };
            }

            return new RateLimitResult { IsAllowed = false, RetryAfter = timestamps.First() + _windowSize - now };
        }
    }
}
