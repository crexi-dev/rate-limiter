using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RateLimiter.Attributes;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter;

public class RateLimiterMiddleware
{
    private const string ApiTokenHeader = "x-api-token";
    private readonly RequestDelegate next;
    private readonly IOptions<RateLimitOptions> options;
    private readonly IEnumerable<IRateLimiterRule> rateLimiterRules;

    public RateLimiterMiddleware(RequestDelegate next,
        IOptions<RateLimitOptions> options,
        IEnumerable<IRateLimiterRule> rateLimiterRules)
    {
        this.next = next;
        this.options = options;
        this.rateLimiterRules = rateLimiterRules;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(ApiTokenHeader, out var tokenValues))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return;
        }

        var token = tokenValues.First();

        var endpoint = context.GetEndpoint();

        var rulesDescriptor = endpoint?.Metadata.GetMetadata<RateRulesAttribute>()?.Rules;
        if (rulesDescriptor is null) return;

        foreach (var ruleDescriptor in rulesDescriptor)
        {
            var rule = rateLimiterRules.Single(i =>
                string.Equals(i.Name, ruleDescriptor, StringComparison.OrdinalIgnoreCase));
            var isAllowed = await rule.IsAllowed(token, options.Value);

            if (isAllowed) continue;

            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            return;
        }

        await next(context);
    }
}