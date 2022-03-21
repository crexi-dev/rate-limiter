using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RateLimiter.API.Extensions;
using RateLimiter.API.Processors;
using RateLimiter.API.Resolvers;
using RateLimiter.Core.Models.Enums;
using RateLimiter.Core.Models.Identity;
using RateLimiter.Core.Models.RateLimit;

namespace RateLimiter.API.Middlewares;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimitOptions _options;
    private readonly IRateLimitProcessor _processor;
    private readonly IIdentityResolver _identityResolver;

    public RateLimitMiddleware(
        RequestDelegate next,
        IOptions<RateLimitOptions> options,
        IRateLimitProcessor processor,
        IIdentityResolver identityResolver)
    {
        _next = next;
        _options = options.Value;
        _processor = processor;
        _identityResolver = identityResolver;
    }

    public async Task Invoke(HttpContext context)
    {
        if (_options is null)
        {
            await _next.Invoke(context);
            return;
        }

        ClientRequestIdentity identity;

        try
        {
            identity = await ResolveIdentity(context);
        }
        catch (Exception)
        {
            await ReturnUnauthorizedResponse(context);
            return;
        }

        IEnumerable<RateLimitRule> rules = _processor.GetMatchingRules(identity);

        foreach (RateLimitRule rule in rules)
        {
            RateLimitCounter rateLimitCounter =
                await _processor.ProcessRequestAsync(identity, rule, context.RequestAborted);

            if (!(rule.Limit > 0)) continue;

            if (rateLimitCounter.Timestamp + rule.PeriodTimespan!.Value < DateTime.UtcNow)
            {
                continue;
            }

            if (!(rateLimitCounter.Count > rule.Limit)) continue;

            await ReturnQuotaExceededResponse(context, rule);
        }
    }

    private Task<ClientRequestIdentity> ResolveIdentity(HttpContext httpContext)
    {
        ClientRequestIdentity identity = _identityResolver.ResolveClient(httpContext);

        string path = httpContext.Request.Path.ToString().ToLowerInvariant();
        identity.Path = path == "/" ? path : path.TrimEnd('/');
        identity.HttpVerb = httpContext.Request.Method.ToLowerInvariant();

        return Task.FromResult(identity);
    }

    private Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule)
    {
        var message =
            $"API calls quota exceeded! maximum admitted {rule.Limit} per" +
            $" {(rule.PeriodTimespan.HasValue ? FormatPeriodTimespan(rule.PeriodTimespan.Value) : rule.Period)}.";

        httpContext.Response.StatusCode = _options.HttpStatusCode;
        httpContext.Response.ContentType = _options.ContentType;

        return httpContext.Response.WriteAsync(message);
    }

    private Task ReturnUnauthorizedResponse(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 401;
        httpContext.Response.ContentType = "text/plain";

        return httpContext.Response.WriteAsync("unauthorized");
    }

    private static string FormatPeriodTimespan(TimeSpan period)
    {
        var sb = new StringBuilder();

        if (period.Days > 0)
        {
            sb.Append($"{period.Days}d");
        }

        if (period.Hours > 0)
        {
            sb.Append($"{period.Hours}h");
        }

        if (period.Minutes > 0)
        {
            sb.Append($"{period.Minutes}m");
        }

        if (period.Seconds > 0)
        {
            sb.Append($"{period.Seconds}s");
        }

        if (period.Milliseconds > 0)
        {
            sb.Append($"{period.Milliseconds}ms");
        }

        return sb.ToString();
    }
}