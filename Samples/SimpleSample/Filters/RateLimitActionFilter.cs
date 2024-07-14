using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RateLimiter;
using RateLimiter.Models;

namespace SimpleSample.Filters;

internal sealed class RateLimitActionFilter : ActionFilterAttribute
{
    private static readonly Dictionary<string, List<RateLimitRule>> RouteRateLimitDictionary =
        new Dictionary<string, List<RateLimitRule>>()
        {
            {
                "/weatherforecast",
                new List<RateLimitRule>()
                {
                    new RateLimitRule(TimeSpan.FromMinutes(1), 5),
                    new RateLimitRule(TimeSpan.FromSeconds(10), 1),
                }
            },
        };

    private readonly IRateLimitingService _rateLimitingService;

    public RateLimitActionFilter(
        IRateLimitingService rateLimitingService)
    {
        _rateLimitingService = rateLimitingService;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        if (RouteRateLimitDictionary.TryGetValue(request.Path, out var rules) == false)
        {
            return;
        }

        var clientId = GetClientId(request);

        if (clientId == null)
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.TooManyRequests);
        }

        var identity = new ClientRequestIdentity(clientId!, request.Path, request.Method);

        foreach (var rule in rules)
        {
            var counter = _rateLimitingService.GetRateLimitCounter(identity, rule);

            SetRateLimitHeaders(context.HttpContext.Response, counter, rule);

            if (counter.TotalRequests > rule.Limit)
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.TooManyRequests);
            }
        }
    }

    private static string? GetClientId(HttpRequest? request)
    {
        return request?.Headers["X-ClientId"].FirstOrDefault();
    }

    private static void SetRateLimitHeaders(HttpResponse response, RateLimitCounter counter, RateLimitRule rule)
    {
        long remainingAttemptes = 0;
        DateTime reset;

        if (counter.TotalRequests > rule.Limit)
        {
            remainingAttemptes = rule.Limit;
            reset = DateTime.UtcNow + rule.Period;
        }
        else
        {
            remainingAttemptes = rule.Limit - counter.TotalRequests;
            reset = counter.StartedAt + rule.Period;
        }

        response.Headers["X-Rate-Limit-Limit"] = rule.Period.ToString();
        response.Headers["X-Rate-Limit-Remaining"] = remainingAttemptes.ToString();
        response.Headers["X-Rate-Limit-Reset"] = reset.ToString();
    }
}
