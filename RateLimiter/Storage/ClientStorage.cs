using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using RateLimiter.Helper;
using RateLimiter.Models;

namespace RateLimiter.Storage
{
    public sealed class ClientStorage
    {
        private static readonly ClientStorage _instance = new ClientStorage();
        
        private readonly MemoryCache _credsCache = new MemoryCache("clientStatistic");
        
        static ClientStorage()
        {
        }

        public static ClientStorage Instance => _instance;

        public ClientStatistic RefreshClientStatistic(string key)
        {
            if (_credsCache.Get(key) is ClientStatistic statistic)
            {
                statistic.Requests.Add(DateTime.UtcNow);
                statistic.RequestCount++;
                var prefix = KeyParser.GetPrefix(key);
                if (statistic.PrefixRequest.TryGetValue(prefix, out var times))
                {
                    times.Add(DateTime.UtcNow);
                }
                else
                {
                    statistic.PrefixRequest.TryAdd(prefix, new List<DateTime> { DateTime.UtcNow });
                }
            }
            else
            {
                var prefix = KeyParser.GetPrefix(key);
                statistic = new ClientStatistic
                {
                    Requests = new List<DateTime> { DateTime.UtcNow }, RequestCount = 1,
                    PrefixRequest = new Dictionary<string, List<DateTime>>
                    {
                        { prefix, new List<DateTime> { DateTime.UtcNow } }
                    }
                };
                _credsCache.Add(key, statistic, new CacheItemPolicy());
            }
            return statistic;
        }
    }
}