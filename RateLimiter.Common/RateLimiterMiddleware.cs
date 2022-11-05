using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using RateLimiter.Models;
using System.Collections.Generic;


namespace RateLimiter
{
    public static class RateLimiterMiddlewareExtensions
    {
        public static IApplicationBuilder UseResourceLimiterMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimiterMiddleware>();
        }
    }
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRulesProcessorService _rulesProcessorService;
        private readonly List<string> _ruleKeys = new();

        public RateLimiterMiddleware(RequestDelegate next, IRulesProcessorService rulesProcessorService)
        {
            _next = next;
            _rulesProcessorService = rulesProcessorService;

        }

        private string GetRequestContext(HttpContext context)
        {
            // Fetch endpoint from request
            var requestPath = context.Request.Path.ToString().ToLowerInvariant().TrimEnd('/');
            return string.IsNullOrWhiteSpace(requestPath) ? "/" : requestPath;
        }

        private string? GetRequestToken(HttpContext context) => context.Request.Headers["Authentication"].FirstOrDefault();
        public async Task InvokeAsync(HttpContext context)
        {

            // Add endpoint as a ruleKey to Request Event Store if its under interest of us
            AddRequestEvent(GetRequestContext(context));


            // Any other parameter from request could be used to be checked on limit. i.e. auth token
            AddRequestEvent(GetRequestToken(context));

            // process rules on rule keys and validate limit

            if (!_rulesProcessorService.IsValidLimit(_ruleKeys))
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsync("Rate Limit Exceeded");
                return;
            }

            await _next(context);
        }

        private void AddRequestEvent(string? requestContext)
        {
            if (string.IsNullOrEmpty(requestContext))
                return;

            _rulesProcessorService.AddRequestEvent(requestContext, new() { RequestDate = DateTimeOffset.UtcNow });
            _ruleKeys.Add(requestContext);
            
        }
    }
}
