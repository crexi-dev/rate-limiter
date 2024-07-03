using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RateLimiter.Interfaces;

namespace RateLimiter.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MinimumTimespanBetweenCallsAttribute : ActionFilterAttribute, IRateLimitingRule
{
    private readonly TimeSpan _timespan;
    private readonly Dictionary<string, DateTime> _lastRequests = new();

    /// <summary>
    /// Request-limiting with a certain timespan passed since the last call rule.
    /// <param name="seconds">Defines a certain timespan passed since the last call.</param>
    /// </summary>
    public MinimumTimespanBetweenCallsAttribute(int seconds)
    {
        _timespan = TimeSpan.FromSeconds(seconds);
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var token = context.HttpContext.Request.Headers["AccessToken"].ToString();
        var resource = context.ActionDescriptor.DisplayName;

        if (!IsRequestAllowed(token, resource))
        {
            context.Result = new ContentResult
            {
                StatusCode = 429,
                Content = "Rate limit exceeded."
            };
        }

        base.OnActionExecuting(context);
    }

    public bool IsRequestAllowed(string token, string resource)
    {
        var key = $"{token}_{resource}";
        var now = DateTime.UtcNow;

        if (_lastRequests.TryGetValue(key, out var lastCall) && now - lastCall < _timespan)
        {
            return false;
        }

        _lastRequests[key] = now;

        return true;
    }
}