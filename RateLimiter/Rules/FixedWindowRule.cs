using System;
using System.Collections.Generic;
using RateLimiter.Models;
using RateLimiter.Rules;

namespace RateLimiter.Rules;

public class FixedWindowRule : IRateLimitRule
{
    private readonly int _maxRequests;
    private readonly TimeSpan _windowSize;
    private readonly Dictionary<string, RateLimitEntry> _clientRequestCounts = new();

    public FixedWindowRule(int maxRequests, TimeSpan windowSize)
    {
        _maxRequests = maxRequests;
        _windowSize = windowSize;
    }

    public RateLimitResult IsRequestAllowed(string clientId)
    {
        lock (_clientRequestCounts)
        {
            DateTime now = DateTime.UtcNow;

            if (!_clientRequestCounts.TryGetValue(clientId, out var entry) || now >= entry.ResetTime)
            {
                _clientRequestCounts[clientId] = new RateLimitEntry { Count = 1, ResetTime = now + _windowSize };
                return new RateLimitResult { IsAllowed = true, RetryAfter = TimeSpan.Zero };
            }

            if (entry.Count < _maxRequests)
            {
                entry.Count++;
                return new RateLimitResult { IsAllowed = true, RetryAfter = TimeSpan.Zero };
            }

            return new RateLimitResult { IsAllowed = false, RetryAfter = entry.ResetTime - now };
        }
    }
}
