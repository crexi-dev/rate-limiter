using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using RateLimiter.Models.Options;
using RateLimiter.Services;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tokenizer;

namespace RateLimiter.Middleware
{
    public class RateLimiterMiddleware
    {
        private readonly IRateLimiterService rateLimiterService;
        private readonly RequestDelegate next;

        public RateLimiterMiddleware(IRateLimiterService rateLimiterService, RequestDelegate next)
        {
            this.rateLimiterService = rateLimiterService;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IOptions<ActiveProcessorsOptions>? apOptions = null)
        {
            var timestamp = DateTime.UtcNow; // Looks like getting context.Timestamp (deprecated) is pretty awful 
            if (apOptions?.Value.ActiveProcessorNames == null || apOptions?.Value.ActiveProcessorNames.Length == 0)
            {
                await next(context);
            }

            // TODO: Public api endpoints would track ip address as the clientId Key in the MemoryCache
            var claims = GetTokenClaims(context);
            var clientId = claims?.ClientId;
            if (string.IsNullOrEmpty(clientId))
            {
                clientId = context.Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? throw new InvalidOperationException("Unable to obatain IP address");
            }

            var rlResponses = rateLimiterService.ProcessRequest(clientId, timestamp);
            if (rlResponses == null || rlResponses.Count == 0)
            {
                await next(context);
            }

            var failedResponses = rlResponses.Where(res => !res.IsSuccess).ToList();
            if (failedResponses.Count > 0)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }

            await next(context);
        }

        private TokenClaims? GetTokenClaims(HttpContext context)
        {
            var bearerToken = context.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(bearerToken))
            {
                return null;
            }

            var claims = AuthHelpers.GetTokenClaims(bearerToken);
            if (string.IsNullOrEmpty(claims.ClientId) || string.IsNullOrEmpty(claims.Region))
            {
                return null;
            }

            return claims;
        }
    }
}
