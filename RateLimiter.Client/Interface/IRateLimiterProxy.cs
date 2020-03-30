using System;

namespace RateLimiter.Client
{
    public interface IRateLimiterProxy {
        bool Verify(string token, DateTime requestDate, string serverIP);
    }
}