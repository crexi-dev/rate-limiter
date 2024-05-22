namespace RateLimiter.Rules;

using Base;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

using Extensions;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Models;

public class TimeSinceLastCallAttribute : RateLimitAttribute
{
    public TimeSinceLastCallAttribute(int timeWindowInSeconds, string countryCode = "")
    {
        TimeWindowInSeconds = timeWindowInSeconds;
        CountryCode = countryCode;
    }

    public int TimeWindowInSeconds { get; set; }

    public override async Task<bool> ValidateAsync(IDistributedCache cache, HttpContext context)
    {
        var consumptionData = await cache.GetCustomerConsumptionDataFromContextAsync(context) ?? new List<ConsumptionData>();
        var result = consumptionData.Count(x => x.ResponseTime > DateTime.UtcNow.AddSeconds(-1 * TimeWindowInSeconds)) == 0;
        return result;
    }
}