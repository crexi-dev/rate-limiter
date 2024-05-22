namespace RateLimiter.Rules;

using Base;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

using Extensions;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Models;
using System.Linq;

public class NumberOfRequestsAttribute : RateLimitAttribute
{
    public NumberOfRequestsAttribute(int timeWindowInSeconds, int maxRequests, string countryCode = "")
    {
        TimeWindowInSeconds = timeWindowInSeconds;
        MaxRequests = maxRequests;
        CountryCode = countryCode;
    }

    public int TimeWindowInSeconds { get; set; }

    public int MaxRequests { get; set; }

    public override async Task<bool> ValidateAsync(IDistributedCache cache, HttpContext context)
    {
        var consumptionData = await cache.GetCustomerConsumptionDataFromContextAsync(context) ?? new List<ConsumptionData>();
        var result = consumptionData.Count(x => x.ResponseTime >= DateTime.UtcNow.AddSeconds(-1 * TimeWindowInSeconds)) < MaxRequests;
        return result;
    }
}