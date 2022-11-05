using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Models;
using System;


namespace RateLimiter.Repository
{
    public class EventsRepository : IEventsRepository
    {
        private readonly IMemoryCache _memoryCache;
        private const string CacheKeyPrefix = "EventsOnRuleKey:";
        private MemoryCacheEntryOptions cacheOptions;
        public EventsRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;


            cacheOptions = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.NeverRemove
            };
        }

        public RequestCollection AddOrReplace(RequestCollection entity)
        {
            return _memoryCache.Set(entity.Key, entity, cacheOptions);
        }

        public RequestCollection GetById(string key)
        {
            key = $"{CacheKeyPrefix}{key}";

            _memoryCache.TryGetValue(key, out RequestCollection requestCollection);

            requestCollection ??= new RequestCollection(key);

            return requestCollection;
        }
    }
}
