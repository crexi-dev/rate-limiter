using System;
using RateLimiter.Services;

namespace RateLimiter.Middleware
{
    public interface IRateLimitRules
    {
        bool IsValidRequestByKey(DateTime? dateTime, string key, int maxrequests, int hitCount);

        bool IsValidRequestByClientToken(DateTime? dateTime, string token, int maxrequests, int hitCount);

    }
}

