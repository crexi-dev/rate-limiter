using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RateLimiter.Interfaces;

namespace RateLimiter.Rules;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class XRequestsPerTimeSpanRuleAttribute<T>(uint maxCount, string timeSpan) : RuleAttribute<T>
    where T : IRuleContextProvider
{
    protected override async Task<bool> IsAllowedAsync(string? clientId, IServiceProvider sp, CancellationToken ct)
    {
        try
        {
            var dt = sp.GetRequiredService<IDateTimeProvider>();
            var now = dt.UtcNow;
        
            var key = $"RateLimiting:XRequestsPerTimeSpanRule:{clientId}";
        
            var store = sp.GetRequiredService<IStore>();
            await using var _ = await store.LockAsync(key, ct);

            var latestEntry = await store.GetAsync<XRequestsPerTimeSpanRuleEntry?>(key, ct);

            // todo: > or >= should be used? Should be discussed with business
            if (latestEntry is null || now - latestEntry.StartingPoint > TimeSpan.Parse(timeSpan))
            {
                await store.SetAsync(key, new XRequestsPerTimeSpanRuleEntry(1, now), ct);
                return true;
            }
        
            if (latestEntry.Count <= maxCount)
            {
                await store.SetAsync(key, latestEntry with { Count = latestEntry.Count + 1 }, ct);
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            var logger = sp.GetRequiredService<ILogger<XRequestsPerTimeSpanRuleAttribute<T>>>();
            logger.LogError(e, "error on rate limit rule");
            // todo: I assume that if store is down for example, we should not block the request, but it should be discussed with business
            return true;
        }

    }
}

public record XRequestsPerTimeSpanRuleEntry(uint Count, DateTime StartingPoint);