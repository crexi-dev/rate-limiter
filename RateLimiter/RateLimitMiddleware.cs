namespace RateLimiter;

using System.Linq;

using Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

using System.Net;
using System.Threading.Tasks;

public class RateLimitMiddleware
{
    private readonly RequestDelegate next;

    private readonly IDistributedCache cache;

    public RateLimitMiddleware(
        RequestDelegate next,
        IDistributedCache cache)
    {
        this.next = next;
        this.cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.HasRateLimitAttribute(out var rateLimitsAttributes))
        {
            await next(context);
            return;
        }

        var consumptionData = await cache.GetCustomerConsumptionDataFromContextAsync(context);
        if (rateLimitsAttributes != null && consumptionData != null)
        {
            if (rateLimitsAttributes.Any(x => !string.IsNullOrWhiteSpace(x.CountryCode)))
            {
                var countryCode = "US"; // replace with fetch data from token
                rateLimitsAttributes = rateLimitsAttributes.Where(x => x.CountryCode == countryCode).ToList();
            }

            foreach (var rateLimitAttribute in rateLimitsAttributes)
            {
                if (!await rateLimitAttribute.ValidateAsync(cache, context))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    return;
                }
            }

            consumptionData.IncreaseRequests(consumptionData.NumberOfRequests + 1);
        }

        await cache.SetCacheValueAsync(context.GetCustomerKey(), consumptionData);

        await next(context);
    }
}