using Microsoft.AspNetCore.Http;
using RateLimiter.Interfaces.Business;
using RateLimiter.Models;
using System.Threading.Tasks;

namespace RateLimiter.Web
{
    /// <summary>
    /// Sample usage of the IRateLimitRulesEngine
    /// </summary>
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimitRulesEngine _rulesEngine;

        public RateLimiterMiddleware(RequestDelegate next, IRateLimitRulesEngine rulesEngine)
        {
            _next = next;
            _rulesEngine = rulesEngine;
        }

        public async Task Invoke(HttpContext context)
        {
            // NOTE: For demo purposes only. A real User instance would need to be supplied here
            var user = new User("383E9902-E3FA-4A08-B77E-3FAA06930922");

            var result = await _rulesEngine.Run(context, user);

            if (!result.Proceed)
            {
                //context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsync(result.Error);

                return;
            }

            await _next(context);
        }
    }
}