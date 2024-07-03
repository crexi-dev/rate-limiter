using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RateLimiter.Interfaces;

namespace RateLimiter.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RateLimitPerTimespanAttribute : ActionFilterAttribute, IRateLimitingRule
{
    private readonly int _maxRequests;
    private readonly TimeSpan _timespan;
    private readonly Dictionary<string, List<DateTime>> _requests = new();

    /// <summary>
    /// Request-limiting with X requests per timespan rule.
    /// <param name="maxRequests">Defines a maximum count of requests.</param>
    /// <param name="seconds">Defines a timespan.</param>
    /// </summary>
    public RateLimitPerTimespanAttribute(int maxRequests, int seconds)
    {
        _maxRequests = maxRequests;
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

        if (!_requests.ContainsKey(key))
        {
            _requests[key] = new List<DateTime>();
        }

        _requests[key].Add(now);
        _requests[key].RemoveAll(timestamp => timestamp < now - _timespan);

        return _requests[key].Count <= _maxRequests;
    }
}