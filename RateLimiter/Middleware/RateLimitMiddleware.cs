using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using RateLimiter.Extensions;

namespace RateLimiter.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimitService _rateLimitService;

        public RateLimitMiddleware(
            RequestDelegate next,
            IRateLimitService rateLimitService)
        {
            _next = next;
            _rateLimitService = rateLimitService;
        }

        public async Task Invoke(HttpContext context)
        {
            // Get identity from request
            var clientId = context.GetClientId();

            var clientRequest = new ClientRequest()
            {
                Resource = context.Request.Path,
                Method = context.Request.Method,
                RequestTime = DateTime.UtcNow,
                ClientId = clientId,
            };

            // Check if client request should be processed or blocked by rate limit rules
            var isRequestValid = await _rateLimitService.ValidateRequest(clientRequest);

            if (!isRequestValid)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            }

            await _next.Invoke(context);
        }
    }
}
