using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{

    public class CacheHelper : ICacheHelper 
    {
        private readonly Dictionary<string, List<DateTime>> _cache = new();
        public int MaxTimes { get; set; } = 60000; // Default: 60 seconds

        public TimeSpan LastRequestTime(string key)
        {
            if (_cache.TryGetValue(key, out var timestamps) && timestamps.Count > 0)
            {
                return DateTime.UtcNow - timestamps[^1];
            }
            return TimeSpan.MaxValue;
        }

        public int RequestsCount(string key, TimeSpan time)
        {
            if (_cache.TryGetValue(key, out var timestamps))
            {
                var threshold = DateTime.UtcNow - time;
                return timestamps.RemoveAll(t => t > threshold);
            }
            return 0;
        }

        public void AddRequest(string key)
        {
            if (!_cache.ContainsKey(key))
            {
                _cache[key] = new List<DateTime>();
            }
            _cache[key].Add(DateTime.UtcNow);
        }
    }
}
