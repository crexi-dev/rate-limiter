using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace RateLimiter;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDistributedCache _cache;

    public RateLimitMiddleware(
        RequestDelegate next,
        IDistributedCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.HasRateLimitAttribute(out var decorator))
        {
            await _next(context);
            return;
        }

        var consumptionData = await _cache.GetCustomerConsumptionDataFromContextAsync(context);
        if (consumptionData is not null)
        {
            if (consumptionData.HasConsumedAllRequests(decorator!.TimeWindowInSeconds, decorator!.MaxRequests))
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }
            
            consumptionData.IncreaseRequests(decorator!.MaxRequests);
        }

        await _cache.SetCacheValueAsync(context.GetCustomerKey(), consumptionData);

        await _next(context);
    }
}