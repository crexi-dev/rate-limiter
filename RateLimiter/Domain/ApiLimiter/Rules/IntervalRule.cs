using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.ApiLimiter
{
    public class IntervalRule : IRule
    {
        private readonly object _lock = new object();

        private readonly ITimestamp _timestamp;
        private readonly long _requestIntervalTicks;
        private readonly int _requestIntervalMs;

        private long? _windowStartTime;

        // In a sliding window we would normally estimate the counts in the previous window
        // TODO: Add in Previous Window estimates

        public IntervalRule(ITimestamp timestamp, int requestIntervalMs)
        {
            _timestamp = timestamp;
            _requestIntervalMs = requestIntervalMs;
            _requestIntervalTicks = requestIntervalMs * TimeSpan.TicksPerMillisecond;
        }

        public bool NewVisitAndRuleCheck()
        {
            lock (_lock)
            {
                var currentTime = _timestamp.GetTimestamp();
                var elapsedTime = _windowStartTime.HasValue ? (currentTime - _windowStartTime.Value) : 0;

                // Window was started previously
                if (_windowStartTime.HasValue)
                {
                    if (elapsedTime < _requestIntervalTicks)
                    {
                        // Within current window? Request failed
                        return false;
                    }
                    // We have reached the end of the current window
                    else
                    {
                        _windowStartTime = currentTime;
                    }
                }
                // This is the first request, start the window
                else
                {
                    _windowStartTime = currentTime;
                }

                return true;
            }
        }

        public object Clone()
        {
            return new IntervalRule(_timestamp, _requestIntervalMs);
        }
    }
}
