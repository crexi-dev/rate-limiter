using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using RateLimiter.RateLimiter.Models;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RateLimiter.Middleware
{
    public class RateLimitingMiddleware
    {
        private const string AuthHeader = "Authorization";
        private const string RegionHeader = "Region";
        private const string RateLimitRemainingHeader = "X-Ratelimit-Remaining";
        private const string RateLimitLimitHeader = "X-Ratelimit-Limit";
        private const string RateLimitRetryAfterHeader = "X-Ratelimit-Retry-After";

        private readonly RequestDelegate _next;
        private readonly RateLimitProcessor _rateLimiter;
        public RateLimitingMiddleware(RequestDelegate next, RateLimitProcessor rateLimiter)
        {
            _next = next;
            _rateLimiter = rateLimiter;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var token = context.Request.Headers[AuthHeader];
            var region = GetRegion(context.Request.Headers[RegionHeader]);

            var enableRateLimitingAttributes = endpoint?.Metadata.GetOrderedMetadata<EnableRateLimitingAttribute>();
            if (enableRateLimitingAttributes is not null && enableRateLimitingAttributes.Any())
            {
                foreach (var enableRateLimitingAttribute in enableRateLimitingAttributes)
                {
                    var result = _rateLimiter.VerifyRequest(
                        token,
                        region,
                        endpoint.DisplayName,
                        enableRateLimitingAttribute.Policies);

                    if (result.Limited is true)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                        context.Response.Headers[RateLimitLimitHeader] = result.CurrentLimit.ToString();
                        context.Response.Headers[RateLimitRemainingHeader] = result.RemainingAmountOfCalls.ToString();
                        if (result.RetryAfterSeconds.HasValue)
                        {
                            context.Response.Headers[RateLimitRetryAfterHeader] = result.RetryAfterSeconds.ToString();
                        }

                        await context.Response.WriteAsync("Rate limit is exceeded. Please try later.");
                        return;
                    }
                }
            }

            await _next(context);
        }

        private Region GetRegion(string regionHeaderValue)
        {
            if (Enum.TryParse(regionHeaderValue, out Region region))
            {
                return region;
            }

            throw new NotSupportedException($"{regionHeaderValue} is not supported.");
        }
    }
}
