using System;
using System.Collections.Generic;

namespace RateLimiter.Rules
{
    internal interface IRateLimiterRule
    {
        bool IsAllowed(IReadOnlyList<DateTimeOffset> userRequests, DateTimeOffset requestDateTime);
    }
}