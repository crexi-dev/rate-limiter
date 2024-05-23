using System;

namespace RateLimiter.Rules.Interfaces;

public interface IRateLimitRule
{
    bool IsRequestAllowed(string clientToken, string resource, DateTime requestTime);
}