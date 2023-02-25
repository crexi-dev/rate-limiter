using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public interface IRateLimitStrategy
    {
        bool IsRequestAllowed(string username, string resource, IRequestRepository requestRepository);
    }
}
