using RateLimiter.Enums;
using RateLimiter.Interfaces;
using System;

namespace RateLimiter.Rules
{
    /// <summary>
    /// This is just an empty template to show a diff request-limiting tecnique
    /// </summary>
    public class TokenBucketRateLimiter : IRateLimiter
    {
        public bool Acquire(ServiceType serviceType, string userToken, DateTime requestedDate)
        {
            throw new NotImplementedException();
        }
    }
}
