using System;

namespace RateLimiter.Domain.ApiLimiter
{
    public class SlidingWindowRule : IRule
    {
        private readonly object _lock = new object();

        private readonly ITimestamp _timestamp;
        private int _requestIntervalMs;
        private readonly long _requestIntervalTicks;
        private readonly int _requestLimit;

        private long? _windowStartTime;
        private int _requestCount;
        public int RequestCount => _requestCount;

        // In a sliding window we would normally estimate the counts in the previous window
        // TODO: Add in Previous Window estimates

        public SlidingWindowRule(ITimestamp timestamp, int requestLimit, int requestIntervalMs)
        {
            _timestamp = timestamp;
            _requestLimit = requestLimit;
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
                    // We have reached the end of the current window
                    if (elapsedTime >= _requestIntervalTicks)
                    {
                        _windowStartTime = currentTime;
                        _requestCount = 0;
                    }
                }
                // This is the first request, start the window
                else
                {
                    _windowStartTime = currentTime;
                }

                if (_requestCount < _requestLimit)
                {
                    _requestCount++;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public object Clone()
        {
            return new SlidingWindowRule(_timestamp, _requestLimit, _requestIntervalMs);
        }
    }
}
