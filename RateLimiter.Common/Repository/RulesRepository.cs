using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Models;
using System;


namespace RateLimiter.Repository
{
    public class RulesRepository : IRulesRepository
    {
        private readonly IMemoryCache _memoryCache;
        private const string CacheKeyPrefix = "ruleOn:";
        private MemoryCacheEntryOptions cacheOptions;
        public RulesRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;


            cacheOptions = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.NeverRemove
            };
        }

        public RuleCollection AddOrReplace(RuleCollection entity)
        {
            string cacheKey = $"{CacheKeyPrefix}{entity.RuleKey}";

            return _memoryCache.Set(cacheKey, entity, cacheOptions);
        }

        RuleCollection IQueryRepository<RuleCollection>.GetById(string key)
        {
            key = $"{CacheKeyPrefix}{key}";
            _memoryCache.TryGetValue(key, out RuleCollection ruleCollection);

            ruleCollection ??= new RuleCollection(key);

            return ruleCollection;
        }
    }
}
