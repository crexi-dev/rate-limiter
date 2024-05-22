namespace RateLimiter.Extensions;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

using Models;

using Newtonsoft.Json;

public static class DistributedCacheExtension
{
    public static async Task<List<ConsumptionData>?> GetCustomerConsumptionDataFromContextAsync(
        this IDistributedCache cache,
        HttpContext context,
        CancellationToken cancellation = default)
    {
        var result = await cache.GetStringAsync(context.GetCustomerKey(), cancellation);
        if (result is null)
            return null;

        return JsonConvert.DeserializeObject<List<ConsumptionData>>(result);
    }

    public static async Task SetCacheValueAsync(
        this IDistributedCache cache,
        string key,
        List<ConsumptionData> customerRequests,
        CancellationToken cancellation = default)
    {
        // If the customer does not yet have consumption data, we will create his record, starting the count at one request
        customerRequests.Add(new ConsumptionData(DateTime.UtcNow));

        await cache.SetStringAsync(key, JsonConvert.SerializeObject(customerRequests), cancellation);
    }
}