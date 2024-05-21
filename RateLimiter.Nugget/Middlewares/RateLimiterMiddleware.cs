using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RateLimiter.Nugget.Middlewares;
using System.Net;

namespace RateLimiter.Nugget.Middlewares
{
    internal sealed class RateLimiterMiddleware<T>
    {
        private readonly RequestDelegate _next;
        private readonly Services.RateLimiter<T> _rateLimiter;

        public RateLimiterMiddleware(RequestDelegate next, Services.RateLimiter<T> rateLimiter)
        {
            _next = next;
            _rateLimiter = rateLimiter;
        }

        public Task Invoke(HttpContext httpContext)
        {

            if (_rateLimiter.IsRateLimitExceeded(httpContext))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return httpContext.Response.WriteAsync("Rate limit exceeded");
            }

            return _next(httpContext);
        }
    }
}

public static class RateLimiterMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiterMiddleware<T>(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimiterMiddleware<T>>();
    }
}
