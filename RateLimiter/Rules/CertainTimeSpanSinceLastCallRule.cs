using System;
using System.Threading.Tasks;
using RateLimiter.Rules.Interfaces;
using RateLimiter.Storage;

namespace RateLimiter.Rules;

public class CertainTimeSpanSinceLastCallRule : IRateLimiterRule
{
    private readonly IStorage<RateLimitEntry> storage;

    public CertainTimeSpanSinceLastCallRule(IStorage<RateLimitEntry> storage)
    {
        this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public string Name => RateRulesEnum.CertainTimeSpanSinceLastCall.ToString();

    public async Task<bool> IsAllowed(string token, RateLimitOptions options)
    {
        var entry = await storage.GetAsync(token) ?? new RateLimitEntry();
        var timeSpanNow = DateTime.UtcNow.TimeOfDay;

        if (entry.LastCall + options.SinceLastCall > timeSpanNow)
        {
            return false;
        }

        entry.LastCall = timeSpanNow;
        await storage.SetAsync(token, entry);

        return true;
    }
}