using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrentTime=> DateTime.UtcNow;
    }
}
