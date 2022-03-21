using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Core.Models.RateLimit;

namespace RateLimiter.API.Store;

public class RateLimitCounterStore : IRateLimitCounterStore
{
    private readonly IMemoryCache _cache;

    public RateLimitCounterStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_cache.TryGetValue(id, out _));
    }

    public Task<RateLimitCounter?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(id, out RateLimitCounter? stored))
        {
            return Task.FromResult(stored);
        }

        return Task.FromResult(default(RateLimitCounter?));
    }

    public Task RemoveAsync(string id, CancellationToken cancellationToken = default)
    {
        _cache.Remove(id);

        return Task.CompletedTask;
    }

    public Task SetAsync(string id, RateLimitCounter? entry, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove
        };

        if (expirationTime.HasValue)
        {
            options.SetAbsoluteExpiration(expirationTime.Value);
        }

        _cache.Set(id, entry, options);

        return Task.CompletedTask;
    }
}