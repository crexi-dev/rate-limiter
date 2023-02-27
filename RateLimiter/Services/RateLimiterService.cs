using System;
using System.Collections.Generic;
using System.Linq;

internal class RateLimiterService : IRateLimiterService
{
    private readonly RateLimiterOptions _options;
    private readonly IRequestCacheService _requestCacheService;

    public RateLimiterService(RateLimiterOptions options, IRequestCacheService requestCacheService)
    {
        _options = options;
        _requestCacheService = requestCacheService;
    }

    public bool Validate(string resource, string token)
    {
        var requestCacheKey = $"{token}-{resource}";
        var attempts = _requestCacheService.GetData<List<DateTime>>(requestCacheKey) ?? new List<DateTime>();
        attempts.Add(DateTime.UtcNow);

        var rules = _options.Get(resource)?.GetRules();

        if (!(rules?.Any() ?? false))
        {
            return true;
        }

        var isValid = rules.All(rule => rule.Validate(attempts));
        if (isValid) _requestCacheService.SetData(requestCacheKey, attempts, TimeSpan.FromDays(1));
        return isValid;
    }
}