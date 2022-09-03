using Microsoft.Extensions.Caching.Memory;

namespace RateLimiter.Stores.MemoryCache
{
    public class MemoryCacheStore : IMemoryCacheStore
    {
        private readonly IMemoryCache memoryCache;
        private const int CacheDuration = 86400; // TODO: revise to what makes more sense
        private static readonly object MemoryLockObject = new object();

        public MemoryCacheStore(IMemoryCache cache)
        {
            this.memoryCache = cache;
        }

        public T Get<T>(string key) where T : class
        {
            return memoryCache.Get(key) as T;
        }

        public void Set<T>(string key, T value) where T : class
        {
            memoryCache.Set(key, value);
        }


    }
}
