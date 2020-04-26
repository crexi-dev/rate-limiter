using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class SlidingWindowStrategy : IRateLimiterStrategy
    {
        public int MaximumRequestQuota { get; private set; }
        public int TimeWindowInSeconds { get; private set; }

        private ConcurrentDictionary<int, Bucket> buckets;
        public SlidingWindowStrategy(int maxRequestPerSecond, int timeWindowInSeconds)
        {
            MaximumRequestQuota = maxRequestPerSecond;
            TimeWindowInSeconds = timeWindowInSeconds;
            buckets = new ConcurrentDictionary<int, Bucket>();
        }

        public bool IsAllowed(int userId, int requestId)
        {
            bool result = false;
            Bucket bucket = buckets.GetOrAdd(userId, new Bucket(userId, MaximumRequestQuota));

            long currentTime = DateTime.UtcNow.Ticks;
            long currentWindowKey = currentTime / TimeSpan.TicksPerSecond * TimeWindowInSeconds;

            int currentCount = bucket.Windows.GetOrAdd(currentWindowKey, 0);
            
            long previousWindowKey = currentWindowKey - TimeWindowInSeconds;
            int previousCount;
            currentCount = bucket.Windows[currentWindowKey]++;
            if (!bucket.Windows.TryGetValue(previousWindowKey, out previousCount))
                result = currentCount <= MaximumRequestQuota;
            else
            {
                double previousCountWeight = 1 - (currentTime - currentWindowKey) / TimeSpan.TicksPerSecond;
                long count = (long)(previousCount * previousCountWeight + currentCount);
                result = count <= MaximumRequestQuota;
            }

            return result;
        }

        class Bucket
        {
            public int Id { get; private set; }
            public ConcurrentDictionary<long, int> Windows { get; private set; }

            public int MaximumRequestQuota { get; private set; }

            public Bucket(int id, int maximumRequestQuota)
            {
                Id = id;
                MaximumRequestQuota = maximumRequestQuota;
                Windows = new ConcurrentDictionary<long, int>();
            }

          
        }
    }

    
}
