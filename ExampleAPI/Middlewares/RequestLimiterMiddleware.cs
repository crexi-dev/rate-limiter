using Microsoft.Extensions.Options;
using RateLimiter.Configuration.Options;
using RateLimiter.Exceptions;
using RateLimiter.Services;

namespace API.Middlewares
{
    public class RequestLimiterMiddleware : IMiddleware
    {
        private readonly LimiterStore _limiterStore;
        private readonly LimiterOptions _limiterOptions;

        private const string LocationQueryParamName = "location";

        public RequestLimiterMiddleware(
            LimiterStore limiterStore, 
            IOptions<LimiterOptions> limiterOptions)
        {
            _limiterStore = limiterStore;
            _limiterOptions = limiterOptions.Value;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var locationName = context.Request.Query[LocationQueryParamName].ToString();
            
            try
            {
                _limiterStore.CheckLocationRequestCount(locationName);
                
                await next(context);
            }
            catch (AllowedRequestsCountReachedException)
            {
                var locationLimitSettings = _limiterOptions
                    .LocationLimiters?
                    .First(_ => _.LocationName == locationName)!;
                
                await context.Response.WriteAsync($"Request limit for {locationName} is {locationLimitSettings.AllowedRequestsCountPerTimeRange} per {locationLimitSettings.TimeRange}");
            }
        }
    }
}