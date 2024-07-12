using System;
using System.Collections.Generic;
using RateLimiter.Model;

namespace RateLimiter.Data
{
    public interface IRequestTrafficDataAccess
    {
        IEnumerable<RateLimitRequest> GetRequests(string token, Uri uri);
    }
}
