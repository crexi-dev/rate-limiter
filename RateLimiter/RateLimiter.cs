using System;
using System.Collections.Generic;

namespace RateLimiter
{
    public class RateLimiterQueue
    {
        private int rate = 1;
        private TimeSpan? duration = null;
        private TimeSpan? period = null;

        private DateTime[] timestamps = new DateTime[1];
        private int lastTimestamp = 0;

        public RateLimiterQueue()
        {
        }

        public RateLimiterQueue EnableRateLimit(int rate, TimeSpan duration)
        {
            if (rate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rate));
            }
            this.timestamps = new DateTime[rate];
            this.rate = rate;
            this.duration = duration;
            return this;
        }

        public RateLimiterQueue DisableRateLimit()
        {
            this.duration = null;
            return this;
        }

        public RateLimiterQueue EnablePeriodLimit(TimeSpan period)
        {
            this.period = period;
            return this;
        }

        public RateLimiterQueue DisablePeriodLimit()
        {
            this.period = null;
            return this;
        }

        public bool Check(DateTime timestamp)
        {
            int index = (this.lastTimestamp + 1) % this.rate;
            if (this.period.HasValue)
            {
                if (timestamp - this.timestamps[this.lastTimestamp] < this.period.Value)
                {
                    return false;
                }
            }
            if (this.duration.HasValue)
            {
                if (timestamp - this.timestamps[index] < this.duration.Value)
                {
                    return false;
                }
            }
            this.timestamps[index] = timestamp;
            this.lastTimestamp = index;
            return true;
        }
    }

    public class RateLimiter<T>
        where T : notnull
    {
        private Func<T, RateLimiterQueue> buildQueue;

        private Dictionary<T, RateLimiterQueue> rateLimits = new();

        public RateLimiter(Func<T, RateLimiterQueue> buildQueue)
        {
            this.buildQueue = buildQueue;
        }

        public bool Check(T token, DateTime timestamp)
        {
            if (!this.rateLimits.ContainsKey(token))
            {
                this.rateLimits.Add(token, this.buildQueue(token));
            }
            return this.rateLimits[token].Check(timestamp);
        }
    }
}
