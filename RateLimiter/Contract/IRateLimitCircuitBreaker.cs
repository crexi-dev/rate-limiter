using System;

namespace RateLimiter.Contract
{
    internal interface IRateLimitCircuitBreaker
    {
        bool IsClientLocked(Guid clientId);

        void LockClient(Guid clientId);
    }
}
