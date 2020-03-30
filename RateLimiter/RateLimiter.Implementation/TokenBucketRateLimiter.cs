using System;

namespace RateLimiter.Implementation
{
    public class TokenBucketRateLimiter {

        //public bool Verify(ClientRequestData clientData,
        //                    RequestsPerTimespanSettings rateLimitSettings) {

        //    var isAllowed = this.Verify(clientData.Count, rateLimitSettings.MaxAmount, rateLimitSettings.RefillAmount, rateLimitSettings.RefillTime, clientData.RequestDate, clientData.LastUpdateDate);
        //    return false;
        //}

        public bool Verify(int count, int maxAmount, int refillAmount, int refillTime, DateTime requestDate, DateTime lastUpdateDate) {
            // refill
            var refillCount = (int) Math.Floor((double)(requestDate - lastUpdateDate).TotalSeconds / refillTime);

            count = Math.Min(maxAmount, count + refillCount * refillAmount);

            // check if token is available
            if (count < 1)
                return false;
            
            return true;
        }
    }
}