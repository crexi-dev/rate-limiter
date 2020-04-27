using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class FixedWindowStrategy : IRateLimiterStrategy
    {
        public int MaximumRequestQuota { get; private set; }
        public int TimeWindowInSeconds { get; private set; }

        private ConcurrentDictionary<int, Bucket> buckets;
        public FixedWindowStrategy(int maxRequestPerWindow, int timeWindowInSeconds)
        {
            MaximumRequestQuota = maxRequestPerWindow;
            TimeWindowInSeconds = timeWindowInSeconds;
            buckets = new ConcurrentDictionary<int, Bucket>();
        }

        public bool IsAllowed(int userId, int requestId)
        {
            bool result = false;
            Bucket bucket = buckets.GetOrAdd(userId, new Bucket(userId, MaximumRequestQuota));

            long currentTime = DateTime.UtcNow.Ticks;
            long ticksFromLastUpdate = currentTime - bucket.LastCountUpdateDT.Ticks;
            long secondsFromLastUpdate = ticksFromLastUpdate / TimeSpan.TicksPerSecond;

            if (secondsFromLastUpdate >= TimeWindowInSeconds)
            {
                bucket.RequestsCount = 1;
                bucket.LastCountUpdateDT = DateTime.UtcNow;
                result = true;
            }
            else
            {
                if (bucket.RequestsCount < MaximumRequestQuota)
                {
                    bucket.RequestsCount++;
                    bucket.LastCountUpdateDT = DateTime.UtcNow;
                    result = true;
                }
                else
                    result = false;
            }

            return result;
        }

        class Bucket
        {
            public int Id { get; private set; }
           

            public int RequestsCount { get; set; }
            public DateTime LastCountUpdateDT { get; set; }
            public int MaximumRequestQuota { get; private set; }

            public Bucket(int id, int maximumRequestQuota)
            {
                Id = id;
                MaximumRequestQuota = maximumRequestQuota;
                LastCountUpdateDT = DateTime.UtcNow;
                RequestsCount = 0;
            }


        }
    }


}
