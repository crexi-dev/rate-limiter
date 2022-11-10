using Microsoft.AspNetCore.Http;
using RateLimiter.Enums;
using RateLimiter.Interfaces;
using RateLimiter.Models.Request;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RateLimiter.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimiterService _rateLimiterService;

        public RateLimitingMiddleware(RequestDelegate next,
            IRateLimiterService rateLimiterService)
        {
            _next = next;
            _rateLimiterService = rateLimiterService;
        }

        public async Task InovkeAsync(HttpContext context)
        {
            ClientRequest request = new()
            {
               Endpoint = context.Request.Path,
               RequestDate = DateTime.UtcNow,
               Location = (Location)context.Items["Location"]
            };

            bool result = await _rateLimiterService.ValidateRequestAsync(
                request.ClientId.ToString(),
                request.RequestDate,
                request.Endpoint,
                request.Location);

            if (!result)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }

            await _next(context);
        }
    }
}
