using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using RateLimiter;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static WeatherForecastApi.Startup;

namespace WeatherForecastApi
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate next_;
        private readonly ServiceResolver serviceResolver_;
        public RateLimitMiddleware(RequestDelegate next, ServiceResolver serviceAccessor)
        {
            next_ = next;
            serviceResolver_ = serviceAccessor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (controllerActionDescriptor is null)
            {
                await next_(context);
                return;
            }

            RateLimitDecorator apiDecorator = (RateLimitDecorator)controllerActionDescriptor.MethodInfo
                            .GetCustomAttributes(true)
                            .SingleOrDefault(w => w.GetType() == typeof(RateLimitDecorator));

            if (apiDecorator is null)
            {
                await next_(context);
                return;
            }

            var key = GenerateClientKey(context);
            if (string.IsNullOrEmpty(key))
            {
                await next_(context);
                return;
            }

            foreach (var strategy in apiDecorator.StrategyTypes)
            {
                IRateLimitHandler limitHandler = serviceResolver_(strategy);
                var isRateLimitSucceded = limitHandler.IsRateLimitSucceded(apiDecorator, key);
                if (isRateLimitSucceded)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    return;
                }

                limitHandler.UpdateClientData(apiDecorator, key);
            }

            await next_(context);
        }

        private static string GenerateClientKey(HttpContext context)
        {
           return context.Request.Headers.TryGetValue("Authorization", out var value)
               ? $"{context.Request.Path}_{value}"
               : string.Empty;
        }
    }
}
