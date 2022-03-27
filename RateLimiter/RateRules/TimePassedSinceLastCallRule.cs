using Microsoft.Extensions.Caching.Memory;
using RateLimiterMy.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.RateRules
{
    public class TimePassedSinceLastCallRule : IRule
    {
        private readonly MemoryCache _Cache = new MemoryCache(new MemoryCacheOptions());
        private readonly MemoryCacheEntryOptions _CacheEntryOptions;
        private readonly TimeSpan _IntervalLimit;
        public TimePassedSinceLastCallRule(TimeSpan intervalLimit)
        {
            _IntervalLimit = intervalLimit;
            _CacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetSlidingExpiration(_IntervalLimit);//Add(new TimeSpan(1, 0, 0))
        }

        public bool Validate(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            lock (_Cache)
            {
                if (_Cache.TryGetValue(request.Token, out DateTime lastRequestTime))
                {

                    if (lastRequestTime + _IntervalLimit > request.Time) return false;
                    else { Add(request); return true; }
                }
                {
                    Add(request);
                    return true;
                }
            }
        }
        private void Add(IRequest request) => _Cache.Set(request.Token, request.Time, _CacheEntryOptions);
    }
}
