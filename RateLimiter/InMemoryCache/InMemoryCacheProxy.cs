using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RateLimiter.InMemoryCache.Interfaces;
using RateLimiter.InMemoryCache.Options;

namespace RateLimiter.InMemoryCache
{
    /// <summary>
    /// A very simple inmemory cache
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InMemoryCacheProxy : IInMemoryCacheProxy
    {
        private readonly IMemoryCache _memoryCache;
        private readonly InMemoryCacheOptions _memoryCacheOptions;

        public InMemoryCacheProxy(IMemoryCache memoryCache,
            IOptions<InMemoryCacheOptions> memoryCacheOptions)
        {
            _memoryCache = memoryCache;
            _memoryCacheOptions = memoryCacheOptions.Value;
        }

        public void AddOrUpdateEntity<T>(string key, T entity)
            where T : class
        {
            _memoryCache.Set(key, entity, _memoryCacheOptions.InspirationTimeSpan);
        }

        public T GetEntity<T>(string key)
            where T : class
        {
            _memoryCache.TryGetValue(key, out T existingEntity);

            return existingEntity;
        }
    }
}
