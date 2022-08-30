using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using RateLimiter.Exceptions;
using RateLimiter.Models.Attributes;
using RateLimiter.Services.Interfaces;

namespace RateLimiter.ApiMiddleware
{
    [ExcludeFromCodeCoverage]
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimiterService _rateLimiterService;

        public RateLimiterMiddleware(RequestDelegate next,
            IRateLimiterService rateLimiterService)
        {
            _next = next;
            _rateLimiterService = rateLimiterService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                var token = httpContext.Request.Headers["Authorization"];
                var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;
                var rateLimiterAttributes = endpoint?.Metadata.OfType<RateLimiterAttribute>().ToList();
                if (rateLimiterAttributes != null && rateLimiterAttributes.Any())
                {
                    await _rateLimiterService.ValidateRateLimitsAsync(token, rateLimiterAttributes.Select(x => x.RateLimiterType).ToList());
                }
                await _next(httpContext);
            }
            catch (RateLimiterFailedException ex)
            {
                httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                await httpContext.Response.WriteAsync(ex.Message);
            }
        }
    }
}
