using RateLimiter.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Handlers
{
    public class TokenBucketHandler : BucketLimiterHandler
    {
        public TokenBucketHandler(RateLimiterSettings rateLimiterSettings) : base(rateLimiterSettings)
        {
            
        }

        public override bool TryProcessRequest(string client)
        {
            var bucket = GetBucket(client);

            bool needUpdateBucket = DateTime.Now.Subtract(bucket.UpdatedAt).TotalSeconds >= bucket.RefreshRate;

            if (bucket.Counter <= 0)
                return false;

            if (needUpdateBucket)
                bucket.UpdateTokens();

            bucket.ProcessRequest();
            return true;
        }
    }
}
