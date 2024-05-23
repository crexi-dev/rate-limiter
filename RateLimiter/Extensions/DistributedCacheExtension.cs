using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RateLimiter.Models;

namespace RateLimiter.Extensions;

public static class DistributedCacheExtension
{
    public static async Task<ConsumptionData?> GetCustomerConsumptionDataFromContextAsync(
        this IDistributedCache cache,
        HttpContext context,
        CancellationToken cancellation = default)
    {
        var result = await cache.GetStringAsync(context.GetCustomerKey(), cancellation);
        return result is null ? null : JsonConvert.DeserializeObject<ConsumptionData>(result);
    }

    public static async Task SetCacheValueAsync(
        this IDistributedCache cache,
        string key,
        ConsumptionData? customerRequests,
        CancellationToken cancellation = default)
    {
        customerRequests ??= new ConsumptionData(DateTime.UtcNow, 1);

        await cache.SetStringAsync(key, JsonConvert.SerializeObject(customerRequests), cancellation);
    }
}