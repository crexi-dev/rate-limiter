using RateLimiter.Decorators;
using RateLimiter.Models;
using RateLimiter.Services;
using System.Net;

namespace RateLimiterApi.Middlewares
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;

        public RateLimitMiddleware(
            RequestDelegate next
            )
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRateLimitService rateLimitService)
        {
            var routeData = context.GetRouteData();
            var controller = routeData.Values["controller"]?.ToString();
            var action = routeData.Values["action"]?.ToString();

            if (string.IsNullOrWhiteSpace(controller) || string.IsNullOrWhiteSpace(action))
            {
                Console.WriteLine($"Can not get controller or action from routes. Requested Url: {context.Request.Path}");
                await _next(context);
                return;
            }            

            var endpoint = context.GetEndpoint();
            
            var decorators = endpoint?.Metadata.GetOrderedMetadata<RateLimitRequest>();

            if (decorators is null || !decorators.Any())
            {
                await _next(context);
                return;
            }

            var validationRequests = new List<RequestAttributeDataModel>();

            foreach (var requestLimits in decorators)
            {
                validationRequests.Add(new RequestAttributeDataModel
                {
                    RequestedMaxRequests = requestLimits.MaxRequests,
                    RequestedType = requestLimits.Type,
                    RequestedRegions = requestLimits.Regions,
                    RequestedTimeWindow = requestLimits.TimeWindow
                });
            }

            // TODO: let's continue without validation.
            var clientId = context.Request.Headers["x-client-id"];
            var region = context.Request.Headers["x-client-region"];
            var key = $"{clientId}.{controller}.{action}";

            if (validationRequests.Any())
            {                
                var result = await rateLimitService.ValidateRequestAsync(key, region, validationRequests);

                if (!result)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    return;
                }
            }

            await rateLimitService.AddClientRequestAsync(key, region);

            await _next(context);
        }
    }
}
