using System;

namespace RateLimiter;

public interface IRateLimitRule
{
    bool IsRequestAllowed(RequestContext context, DateTime requestTime);
}