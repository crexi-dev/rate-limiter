using System;

namespace RateLimiter.Library.Algorithms
{
    public class TimespanPassedSinceLastCallRateLimiter : ITimespanPassedSinceLastCallRateLimiter {
       public bool VerifyTimespanPassedSinceLastCall(DateTime requestDate, TimeSpan timeSpanLimit, DateTime lastUpdateDate) {
            if (requestDate < lastUpdateDate.Add(timeSpanLimit))
                return true;

            return false;
        }
    }
}