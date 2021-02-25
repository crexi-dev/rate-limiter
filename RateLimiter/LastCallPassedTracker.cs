using System;

namespace RateLimiter
{
    public class LastCallPassedTracker : ITracker
    {
        private DateTime LastRequest { get; set; }
        private readonly Period period;
        private readonly long ruleValue;

        public LastCallPassedTracker(Period period, long ruleValue)
        {
            this.period = period;
            this.ruleValue = ruleValue;
        }

        public bool Track()
        {
            var now = DateTime.UtcNow;
            if (LastCallPlusPeriodTime(period, ruleValue) < now)
            {
                LastRequest = now;
                return true;
            }
            return false;
        }

        private DateTime LastCallPlusPeriodTime(Period period, long value)
        {
            switch (period)
            {
                case Period.Millisecond:
                    return LastRequest.AddMilliseconds(value);
                case Period.Second:
                    return LastRequest.AddSeconds(value);
                case Period.Minute:
                    return LastRequest.AddMinutes(value);
                case Period.Hour:
                    return LastRequest.AddHours(value);
                case Period.Day:
                    return LastRequest.AddDays(value);
                default:
                    return LastRequest;
            }
        }
    }
}
