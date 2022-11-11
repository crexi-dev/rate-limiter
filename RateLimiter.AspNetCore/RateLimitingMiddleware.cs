using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RateLimiter.AspNetCore.Attributes;

namespace RateLimiter.AspNetCore;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimitingOptions _options;
    private readonly IRateLimiterStorage _storage;

    public RateLimitingMiddleware(RequestDelegate next, IOptions<RateLimitingOptions> options, IRateLimiterStorage storage)
    {
        _next = next;
        _storage = storage;
        _options = options.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        var now = DateTime.UtcNow;
        var endpoint = context.GetEndpoint();

        if (endpoint is null || endpoint?.Metadata.GetMetadata<IgnoreRateLimiting>() is not null)
        {
            await _next(context);
            return;
        }

        var rateLimitingConfigurations = endpoint?.Metadata.GetOrderedMetadata<RateLimitingAttribute>();

        if (rateLimitingConfigurations is null || rateLimitingConfigurations.Count == 0)
        {
            await _next(context);
            return;
        }

        var rules = rateLimitingConfigurations.Select(x => new RateLimiterRule(x.Interval, x.Limit, x.Parameters))
            .ToList();

        var rateLimiter = new RateLimiter(_storage, rules);

        //TODO: Split Providers for Resource Identifier and Parameters
        var user = _options.ParameterProviders[RateLimitingParameters.User].Resolve(context);

        var region = _options.ParameterProviders[RateLimitingParameters.Region].Resolve(context);

        var succeed = await rateLimiter.Try($"{endpoint.DisplayName}_{user}",
            new Attempt(now, new[] { new RateLimiterParameter(RateLimitingParameters.Region, region) }));

        if (succeed)
        {
            await _next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
    }
}