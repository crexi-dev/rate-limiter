using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class EuRule : IRule
    {
        public TimeSpan Interval { get; set; }

        public bool IsExceeded(string requestKey)
        {
            bool result = false;

            var cacheItem = MemoryCache.Default.Get(requestKey);
            if (cacheItem == null)
            {
                MemoryCache.Default.Set(requestKey, true, DateTime.Now.Add(Interval));
                result = false;
            }
            else
            {
                MemoryCache.Default.Set(requestKey, true, DateTime.Now.Add(Interval));
                result = true;

            }

            return result;

        }
    }
}
