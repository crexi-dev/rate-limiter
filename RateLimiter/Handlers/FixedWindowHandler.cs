using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RateLimiter.Handlers
{

    public class FixedWindowHandler : BucketLimiterHandler
    {
        public FixedWindowHandler(RateLimiterSettings rateLimiterSettings) : base(rateLimiterSettings)
        {
            
        }

        public override bool TryProcessRequest(string clientId)
        {
            var windowSize = rateLimiterSettings.RefreshRate;
            TimeSpan timeSpan = TimeSpan.FromSeconds(windowSize);
            var currentDate = DateTime.Now;

            var startTime = currentDate.RoundDown(timeSpan);
            var endTime = currentDate.RoundUp(timeSpan);
            
            var bucket = GetBucket(clientId);
            if (bucket.UpdatedAt < startTime)
                bucket.UpdateTokens();
            if (bucket.Counter <= 0)
                return false;
            bucket.ProcessRequest();
            return true;
        }
    }
}
