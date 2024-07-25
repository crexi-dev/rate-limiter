using RateLimiter.Enums;
using System;

namespace RateLimiter.Interfaces
{
    public interface IRateLimiter
    {
        public bool Acquire(ServiceType serviceType, string userToken, DateTime requestedDate);
    }
}
