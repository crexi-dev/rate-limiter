using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{

    public enum EnumRestoreRateTimePeriod { Seconds = 0, Minutes = 1, Hours = 2, Days = 3}

    public class LeakyBucketStrategy : IRateLimiterStrategy
    {
       
        //The amount of time between request quota restores, up to the maximum request quota.
        public int RestoreRateTimeAmount { get; private set; }

        // Time period for Restore Rate
        public EnumRestoreRateTimePeriod RestoreRateTimePeriod { get; private set; }

        // The amount of requested restored for restore rate, up to the maximum request quota.
        public int RestoreRateAmount { get; private set; }

        // The maximum size that the request quota can reach.
        public int MaximumRequestQuota { get; private set; }


        private ConcurrentDictionary<int, Bucket> buckets;

        public LeakyBucketStrategy(int requestQuota, int maximumRequestQuota, int restoreRateAmount, int restoreRateTimeAmount, EnumRestoreRateTimePeriod restoreRateTimePeriod)
        {
            MaximumRequestQuota = maximumRequestQuota;
            RestoreRateAmount = restoreRateAmount;
            RestoreRateTimePeriod = restoreRateTimePeriod;
            RestoreRateTimeAmount = restoreRateTimeAmount;
            buckets = new ConcurrentDictionary<int, Bucket>();
        }

        public bool ApplyStrategy(int userId, int requestId)
        {
            bool result = false;

            Bucket bucket =  buckets.GetOrAdd(userId, new Bucket(userId, MaximumRequestQuota));

            applyRestoreRate(bucket);

            if (bucket.RequestQuota > 0)
            {
                bucket.Add(requestId);
                result = true;
            }
                
            return result;
        }

        private void applyRestoreRate(Bucket bucket)
        {
            long ticksFromLastUpdate = DateTime.UtcNow.Ticks - bucket.LastQueueUpdate.Ticks;
            int timeFromLastUpdate;
            int requestQuotaToRestore = 0;
            switch(RestoreRateTimePeriod)
            {
                case EnumRestoreRateTimePeriod.Seconds:
                    timeFromLastUpdate = Convert.ToInt32(ticksFromLastUpdate / TimeSpan.TicksPerSecond);
                    break;
                case EnumRestoreRateTimePeriod.Minutes:
                    timeFromLastUpdate = Convert.ToInt32(ticksFromLastUpdate / TimeSpan.TicksPerMinute);
                    break;
                case EnumRestoreRateTimePeriod.Hours:
                    timeFromLastUpdate = Convert.ToInt32(ticksFromLastUpdate / TimeSpan.TicksPerHour);
                    break;
                case EnumRestoreRateTimePeriod.Days:
                    timeFromLastUpdate = Convert.ToInt32(ticksFromLastUpdate / TimeSpan.TicksPerDay);
                    break;
                default:
                    timeFromLastUpdate = Convert.ToInt32(ticksFromLastUpdate / TimeSpan.TicksPerSecond);
                    break;
            }

            requestQuotaToRestore = RestoreRateAmount * (timeFromLastUpdate / RestoreRateTimeAmount);

            bucket.Remove(requestQuotaToRestore);

        }

    }

    class Bucket
    {
        public int Id { get; private set; }
        private ConcurrentQueue<int> requestsQueue;
        public DateTime LastQueueUpdate { get; set; }
        public int RequestQuota { get; set; }

        public int MaximumRequestQuota { get; private set; }

        public Bucket(int id, int maximumRequestQuota)
        {
            Id = id;
            MaximumRequestQuota = maximumRequestQuota;
            RequestQuota = maximumRequestQuota;
            requestsQueue = new ConcurrentQueue<int>();
            LastQueueUpdate = DateTime.UtcNow;
        }

        public void Add(int requestId)
        {
            requestsQueue.Enqueue(requestId);
            RequestQuota--;
            LastQueueUpdate = DateTime.UtcNow;
        }

        public void Remove(int amount)
        {
            int removeAmount = RequestQuota + amount > MaximumRequestQuota ? MaximumRequestQuota : amount;

            for (int i = 0; i < removeAmount; i++)
            {
                requestsQueue.TryDequeue(out _);
                RequestQuota++;
             }
            LastQueueUpdate = DateTime.UtcNow;
        }
    }
}
