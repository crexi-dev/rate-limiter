using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Types
{
    public enum RateLimiterType
    {
        None = 0,
        NumberRequestsPerTime = 1,
        TimeBetweenTwoRequests = 2
    }
}
