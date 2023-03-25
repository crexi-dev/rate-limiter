using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Handlers
{
    public class BucketLimiterHandler : ILimiterHandler
    {
        public BucketLimiterHandler(RateLimiterSettings rateLimiterSettings)
        {
            this.rateLimiterSettings = rateLimiterSettings;
        }

        protected ILimiterBucket GetBucket(string client)
        {
            if (!clientBuckets.ContainsKey(client))
                clientBuckets.Add(client, new LimiterBucket(rateLimiterSettings.Capacity, rateLimiterSettings.RefreshRate));

            return clientBuckets[client];
        }

        protected Dictionary<string, ILimiterBucket> clientBuckets = new Dictionary<string, ILimiterBucket>();

        protected readonly RateLimiterSettings rateLimiterSettings = new RateLimiterSettings(0, 0);

        public virtual bool TryProcessRequest(string client)
        {
            throw new NotImplementedException();
            //var bucket = GetBucket(client);

            //if (bucket.Counter > 0)
            //    return true;
            //return false;
        }
    }
}
