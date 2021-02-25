using System;

namespace RateLimiter
{
    public class RequestPerPeriodTracker : ITracker
    {
        private readonly Period period;
        private readonly long ruleValue;
        private DateTime LastRequest { get; set; }
        private long PeriodRequestCount { get; set; }

        public RequestPerPeriodTracker(Period period, long ruleValue)
        {
            this.period = period;
            this.ruleValue = ruleValue;
        }

        public bool Track()
        {
            var now = DateTime.UtcNow;
            PeriodRequestCount++;
            if (PeriodRequestCount > ruleValue)
            {
                return false;
            }

            if (LastRequestPeriodAdjusted(period) > now)
            {
                LastRequest = now;
                PeriodRequestCount = 1;
            }
            return true;
        }

        private DateTime LastRequestPeriodAdjusted(Period period)
        {
            switch (period)
            {
                case Period.Millisecond:
                    return LastRequest.AddMilliseconds(1);
                case Period.Second:
                    return LastRequest.AddSeconds(1);
                case Period.Minute:
                    return LastRequest.AddMinutes(1);
                case Period.Hour:
                    return LastRequest.AddHours(1);
                case Period.Day:
                    return LastRequest.AddDays(1);
                default:
                    return LastRequest;
            }
        }
    }
}
