using Microsoft.AspNetCore.Http;
using RateLimiter.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IEnumerable<IRateLimitRule> _rateLimitRules;

    public RateLimitMiddleware(RequestDelegate next, IEnumerable<IRateLimitRule> rateLimitRules)
    {
        _next = next;
        _rateLimitRules = rateLimitRules;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var route = context.Request.Path.ToString();
        var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var token = new AccessToken(clientId);

        if (_rateLimitRules.Any(rule => !rule.IsRequestAllowed(token, route)))
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
            return;
        }

        await _next(context);
    }
}
