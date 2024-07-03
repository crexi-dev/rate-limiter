using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RateLimiter.Interfaces;

namespace RateLimiter.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RegionBasedRateLimitAttribute : ActionFilterAttribute, IRateLimitingRule
{
    private readonly IRateLimitingRule _usRule;
    private readonly IRateLimitingRule _ukRule;

    /// <summary>
    /// Request-limiting with a region-based rule. For US-based tokens - X requests per timespan, for EU-based - certain timespan passed since the last call.
    /// <param name="usMaxRequests">Defines a maximum count of requests.</param>
    /// <param name="usSeconds">Defines a timespan.</param>
    /// <param name="ukSeconds">Defines a certain timespan passed since the last call.</param>
    /// </summary>
    public RegionBasedRateLimitAttribute(int usMaxRequests, int usSeconds, int ukSeconds)
    {
        _usRule = new RateLimitPerTimespanAttribute(usMaxRequests, usSeconds);
        _ukRule = new MinimumTimespanBetweenCallsAttribute(ukSeconds);
    }

    public bool IsRequestAllowed(string token, string resource)
    {
        // US-based tokens
        if (token.StartsWith("US"))
        {
            return _usRule.IsRequestAllowed(token, resource);
        }

        // UK-based tokens
        if (token.StartsWith("UK"))
        {
            return _ukRule.IsRequestAllowed(token, resource);
        }

        return true;
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
}