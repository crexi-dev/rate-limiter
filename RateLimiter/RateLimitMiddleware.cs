namespace RateLimiter;

using System.Collections.Generic;
using System.Linq;

using Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

using System.Net;
using System.Threading.Tasks;

using Models;
using RateLimiter.Storage;

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

        var consumptionData = await cache.GetCustomerConsumptionDataFromContextAsync(context) ?? new List<ConsumptionData>();
        if (rateLimitsAttributes != null)
        {
            if (rateLimitsAttributes.Any(x => !string.IsNullOrWhiteSpace(x.CountryCode)))
            {
                var countryCode = CountryCodeStorage.CountryCode; // replace with fetch data from token
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
        }

        await cache.SetCacheValueAsync(context.GetCustomerKey(), consumptionData);

        await next(context);
    }
}