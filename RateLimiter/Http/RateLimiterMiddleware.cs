using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RateLimiter.Abstractions;
using RateLimiter.Configuration;

namespace RateLimiter.Http
{
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRequestContextFactory _contextFactory;
        
        public RateLimiterMiddleware(RequestDelegate next, IRequestContextFactory contextFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }
        
        public async Task InvokeAsync(HttpContext context, IOptionsMonitor<RateLimitOptions> options)
        {
            var requestContext = _contextFactory.Create(context);
            var currentOptions = options.CurrentValue;
            
            var rules = currentOptions.GetRulesForEndpoint(requestContext.ResourcePath);
            
            foreach (var rule in rules)
            {
                var isAllowed = await rule.ValidateAsync(requestContext);
                
                if (!isAllowed)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.Headers.Add("Retry-After", "1");
                    await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                    return;
                }
            }
            
            await _next(context);
        }
    }
}