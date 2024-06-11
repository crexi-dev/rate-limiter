using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;

namespace RateLimiter.Rules;
public class FixedWindowLimiterRule(TimeProvider timeProvider) : IRule
{
    private FixedWindowLimiterRuleConfiguration? _configuration;

    private readonly Dictionary<string, FixedWindow> windowPerUser = [];

    public FixedWindowLimiterRule Configure(FixedWindowLimiterRuleConfiguration configuratuion)
    {
        _configuration = configuratuion;
        return this;
    }

    public bool IsRequestAllowed(string token)
    {
        if (_configuration is null)
        {
            throw new ArgumentException($"Call {nameof(Configure)} method to configure this rule first");
        } 

        var currentTime = timeProvider.GetUtcNow();

        var currentWindow = windowPerUser.GetValueOrDefault(token);

        // If there is a new user OR it is time to start a new window,
        // initialize a new fixed window with the current request time.
        if (currentWindow is null
        || currentWindow.Start + _configuration.Window < currentTime)
        {
            currentWindow = new FixedWindow(currentTime, 0);
        }


        // If a number of requests within the window exceeds the limit,
        // disallow this request. Otherwise, update the current request count
        // and allow the request.
        if (currentWindow.Count >= _configuration.Limit)
        {
            return false;
        }
        else
        {
            var updatedWindow = currentWindow with { Count = currentWindow.Count + 1 };
            windowPerUser[token] = updatedWindow;
            return true;
        }
    }
}

public record FixedWindowLimiterRuleConfiguration(TimeSpan Window, int Limit);

internal record FixedWindow(DateTimeOffset Start, int Count);