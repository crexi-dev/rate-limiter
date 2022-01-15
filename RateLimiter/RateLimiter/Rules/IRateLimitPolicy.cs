using RateLimiter.Domain;
using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimiter.Rules
{
    public interface IRateLimitPolicy
    {
        bool Check(string accessToken, IEnumerable<UserRequest> userRequests, DateTime currentDate);
    }
}
