using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{

    public enum EnumRestoreRateTimePeriod { Seconds = 0, Minutes = 1, Hours = 2, Days = 3}
    public class LeakyBucketStrategy : IRateLimiterStrategy
    {
        //The number of requests that you can submit at one time without throttling. The request quota decreases with each request and increases at the restore rate.
        public int RequestQuota { get; private set; }

        //The amount of time between request quota restores, up to the maximum request quota.
        public int RestoreRateTimeAmount { get; private set; }

        // Time period for Restore Rate
        public EnumRestoreRateTimePeriod RestoreRateTimePeriod { get; private set; }

        // The amount of requested restored for restore rate, up to the maximum request quota.
        public int RestoreRateAmount { get; private set; }

        // The maximum size that the request quota can reach.
        public int MaximumRequestQuota { get; private set; }
      
        public LeakyBucketStrategy(int requestQuota, int maximumRequestQuota, int restoreRateAmount, int restoreRateTimeAmount, EnumRestoreRateTimePeriod restoreRateTimePeriod)
        {
            RequestQuota = requestQuota;
            MaximumRequestQuota = maximumRequestQuota;
            RestoreRateAmount = restoreRateAmount;
            RestoreRateTimePeriod = restoreRateTimePeriod;
            RestoreRateTimeAmount = restoreRateTimeAmount;
        }

        public bool ApplyStrategy()
        {
            throw new NotImplementedException();
        }
    }
}
