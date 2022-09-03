using System;
using System.Collections.Generic;

namespace RateLimiter.Repositories
{
    public interface IClientRequestRepository
    {
        List<DateTime> Add(string clientId, DateTime requestTime);
    }
}
