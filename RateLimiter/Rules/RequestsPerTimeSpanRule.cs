using System;
using System.Threading.Tasks;
using RateLimiter.Rules.Interfaces;
using RateLimiter.Storage;

namespace RateLimiter.Rules;

public class RequestsPerTimeSpanRule : IRateLimiterRule
{
    private readonly IStorage<RateLimitEntry> storage;

    public RequestsPerTimeSpanRule(IStorage<RateLimitEntry> storage)
    {
        this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public string Name => RateRulesEnum.RequestsPerTimeSpanRules.ToString();

    public async Task<bool> IsAllowed(string token, RateLimitOptions options)
    {
        var entry = await storage.GetAsync(token) ?? new RateLimitEntry();

        if (entry.CallsCount > options.CallsCountLimit)
        {
            return false;
        }

        ++entry.CallsCount;
        await storage.SetAsync(token, entry);

        return true;
    }
}