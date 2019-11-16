using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace RateLimiter
{
    public class LimiterCache
    {
        #region - Variables -
        private MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        #endregion


        #region - Public Methods - 
        public RateLimiter GetOrCreate(object key, TimeSpan period, int maxAction, TimeSpan? minTimeBetweenActions = null)
        {
            RateLimiter cacheEntry;

            if(!_cache.TryGetValue(key, out cacheEntry))
            {
                cacheEntry = new RateLimiter(maxAction, minTimeBetweenActions);
                _cache.Set(key, cacheEntry,period);
            }

            return cacheEntry;
        }
        #endregion

    }
}
