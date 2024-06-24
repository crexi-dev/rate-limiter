using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace RateLimiter;

public class RateLimitingMiddleware
{
    private const string LOCATION_HEARED = "Location";
    private const string AUTH_HEADER = "Authorization";
    private readonly RequestDelegate _next;
    private readonly RateLimiter _rateLimiter;

    public RateLimitingMiddleware(RequestDelegate next, RateLimiter rateLimiter)
    {
        _next = next;
        _rateLimiter = rateLimiter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var resource = context.Request.Path.Value;
        var location = context.Request.Headers[LOCATION_HEARED];
        var token = context.Request.Headers[AUTH_HEADER];

        if (!_rateLimiter.IsRequestAllowed(resource, location, token))
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded.");
            return;
        }

        await _next(context);
    }
}