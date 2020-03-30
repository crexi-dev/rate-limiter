using System;

namespace RateLimiter.Implementation
{
    public class TimespanPassedRateLimiter : ITimespanPassedRateLimiter {
       public bool Verify(DateTime requestDate, TimeSpan timeSpanLimit, DateTime lastUpdateDate) {
            if (requestDate < lastUpdateDate.Add(timeSpanLimit))
                return true;

            return false;
        }
    }
}