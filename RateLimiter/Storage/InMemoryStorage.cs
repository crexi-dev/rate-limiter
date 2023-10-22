using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace RateLimiter.Storage;

public class InMemoryStorage<T> : IStorage<T>
{
    private readonly IMemoryCache cache;

    public InMemoryStorage(IMemoryCache cache)
    {
        this.cache = cache;
    }

    public Task<T?> GetAsync(string id)
    {
        if (cache.TryGetValue(id, out T? stored))
        {
            return Task.FromResult(stored);
        }

        return Task.FromResult(default(T));
    }

    public Task SetAsync(string id, T entry)
    {
        var options = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove
        };

        cache.Set(id, entry, options);

        return Task.CompletedTask;
    }
}