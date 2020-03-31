using System;
using RateLimiter.Library;

namespace RateLimiter
{
    public interface IRateLimiter
    {
        bool Verify(string token, DateTime requestDate, RateLimitSettingsConfig rateLimitSettingsConfig = null);
    }
}