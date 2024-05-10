using Microsoft.AspNetCore.Http;
using RateLimiter.Processor;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimitProcessor _processor;

        public RateLimiterMiddleware(RequestDelegate next, IRateLimitProcessor processor)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var result = _processor.Process(httpContext);

            if (!result)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }


            await _next(httpContext);
        }
    }
}
