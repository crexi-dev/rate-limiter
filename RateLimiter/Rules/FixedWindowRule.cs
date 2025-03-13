using System;
using System.Collections.Generic;

namespace RateLimiter.Rules;

public class FixedWindowRule : IRateLimitRule
{
    private readonly int _maxRequests;
    private readonly long _windowSizeMs;
    private readonly Dictionary<string, int> _requestCounts = new();
    private readonly Dictionary<string, long> _windowStartTimes = new();

    public FixedWindowRule(int maxRequests, TimeSpan windowSize)
    {
        _maxRequests = maxRequests;
        _windowSizeMs = (long)windowSize.TotalMilliseconds;
    }

    public bool AllowRequest(string clientId)
    {
        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (!_windowStartTimes.ContainsKey(clientId))
        {
            _windowStartTimes[clientId] = currentTime;
            _requestCounts[clientId] = 0;
        }

        long windowStartTime = _windowStartTimes[clientId];

        if (currentTime - windowStartTime >= _windowSizeMs)
        {
            _windowStartTimes[clientId] = currentTime;
            _requestCounts[clientId] = 0;
        }

        if (_requestCounts[clientId] < _maxRequests)
        {
            _requestCounts[clientId]++;
            return true;
        }

        return false;
    }
}
