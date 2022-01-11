using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RateLimiter.Models;

namespace RateLimiter
{
    public class RateLimiterMiddleware
    {
        private readonly RateLimitOptions _options;
        private readonly RequestDelegate _next;

        public RateLimiterMiddleware(RequestDelegate next, RateLimitOptions options)
        {
            _options = options;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _options.LimitRules.ForEach(x => x.CollectRequests(context));

            var pass = _options.LimitRules.TrueForAll(x => x.ExecuteRule(context));

            if (pass)
            {
                await _next.Invoke(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            }
        }
    }
}
