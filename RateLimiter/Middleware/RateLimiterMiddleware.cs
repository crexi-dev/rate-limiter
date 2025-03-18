using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RateLimiter.Services;

namespace RateLimiter.Middleware;

/// <summary>
/// Middleware for rate limiting requests based on predefined rules.
/// </summary>
public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimiterService _rateLimiter;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimiterMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="rateLimiter">The rate limiter service.</param>
    public RateLimiterMiddleware(RequestDelegate next, IRateLimiterService rateLimiter)
    {
        _next = next;
        _rateLimiter = rateLimiter;
    }

    /// <summary>
    /// Invokes the middleware to check the rate limit for the current request.
    /// </summary>
    /// <param name="context">The HTTP context of the current request.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        string resource = context.Request.Path.ToString();
        string token = context.Request.Headers["X-Client-Token"].FirstOrDefault() ?? string.Empty;

        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
        else if (!_rateLimiter.IsRequestAllowed(resource, token))
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Request limit exceeded.");
            return;
        }

        await _next(context);
    }
}