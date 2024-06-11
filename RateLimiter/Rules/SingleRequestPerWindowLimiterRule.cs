using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;

namespace RateLimiter.Rules;
public class SingleRequestPerWindowLimiterRule(TimeProvider timeProvider): IRule
{
    internal TimeSpan? _requestTimeSpanLimit;

    private readonly Dictionary<string, DateTimeOffset> usersLastCall = [];

    public SingleRequestPerWindowLimiterRule Configure(TimeSpan requestTimeSpanLimit)
    {
        _requestTimeSpanLimit = requestTimeSpanLimit;
        return this;
    }

    public bool IsRequestAllowed(string token)
    {
        if (_requestTimeSpanLimit is null)
        {
            throw new ArgumentException($"Call {nameof(Configure)} method to configure this rule first");
        }

        var currentTime = timeProvider.GetUtcNow();

        if (usersLastCall.TryGetValue(token, out var lastCall) && lastCall + _requestTimeSpanLimit >= currentTime)
        {
            return false;
        }
        else
        {
            usersLastCall[token] = currentTime;
        }

        return true;
    }
}
