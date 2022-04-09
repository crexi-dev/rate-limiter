using RateLimiter.DataStorageSimulator;
using RateLimiter.Interfaces;

namespace RateLimiter.Validators
{
    /// <summary>
    /// Class-Factory which is creates Rate Limiter Validators based on the Rare Limiter Type
    /// </summary>
    class RateLimiterValidatorFactory : IRateLimiterValidatorFactory
    {
        /// <summary>
        /// Returns Rate Limiter Validator based on the Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IRateLimiterValidator CreateRateLimiterValidator(RateLimiterType type)
        {
            switch (type) 
            {
                case RateLimiterType.rlRequestsPerInterval: 
                {
                    return new RateLimiterValidatorRequestsPerIntervalRule();
                }
                case RateLimiterType.rlRequestsPerTimeout:
                {
                    return new RateLimiterValidatorRequestsPerTimeoutRule();
                }
                default: 
                {
                    return null;
                }
            }
        }
    }
}
