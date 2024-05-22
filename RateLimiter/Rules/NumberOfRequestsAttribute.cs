namespace RateLimiter.Rules;

using Base;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

using Extensions;

using System;
using System.Threading.Tasks;

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
        var consumptionData = await cache.GetCustomerConsumptionDataFromContextAsync(context);
        var result = consumptionData != null && DateTime.UtcNow < consumptionData.LastResponse.AddSeconds(TimeWindowInSeconds) && consumptionData.NumberOfRequests < MaxRequests;
        return result;
    }
}