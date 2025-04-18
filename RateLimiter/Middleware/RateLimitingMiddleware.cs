using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RateLimiter.Core;
using System.Collections.Generic;
using System;

namespace RateLimiter.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Core.RateLimiter _rateLimiter;

        public RateLimitingMiddleware(RequestDelegate next, Core.RateLimiter rateLimiter)
        {
            _next = next;
            _rateLimiter = rateLimiter;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // get client token
            string clientToken = GetClientToken(context);

            // get resource identifier
            string resourceId = context.Request.Path;

            // check if request is allowed
            if (_rateLimiter.IsRequestAllowed(clientToken, resourceId))
            {
                // continue to next middleware
                await _next(context);
            }
            else
            {
                // request is rate limited
                context.Response.StatusCode = 429; // Too Many Requests!
                context.Response.Headers.Append("Retry-After", "60"); // suggest retry after 60 seconds
                await context.Response.WriteAsync("Rate limit exceeded..please go away");
            }
        }

        private string GetClientToken(HttpContext context)
        {
            // check for a region header
            string region = "US"; // default
            if (context.Request.Headers.TryGetValue("X-Region", out var regionHeader))
            {
                region = regionHeader;
            }

            // get from API key header
            if (context.Request.Headers.TryGetValue("X-API-Key", out var apiKey))
            {
                return $"{region}-{apiKey}";
            }

            // or use IP address
            return $"{region}-{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";
        }
    }
}