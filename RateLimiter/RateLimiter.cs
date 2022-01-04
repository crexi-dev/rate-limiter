using RateLimiter.TimeStamp;
using System;

namespace RateLimiter
{
    /// <summary>
    /// Rate limiting using Sliding window algorithm
    /// </summary>
    public class RateLimiter
    {
        private readonly object _syncObject = new object();

        private readonly ITimestamp _timestamp;
        private readonly long _requestIntervalTicks;
        private readonly int _requestLimit;

        private long? _windowStartTime;
        private int _prevRequestCount;
        private int _requestCount;

        public RateLimiter(ITimestamp timestamp, int requestLimit, int requestIntervalMs)
        {
            if (requestLimit < 1)
            {
                throw new ArgumentException($"Received invalid value for {nameof(requestLimit)}, {requestLimit}. Input must be one or greater.");
            }

            if (requestIntervalMs <= 0)
            {
                throw new ArgumentException($"Received invalid value for {nameof(requestIntervalMs)}, {requestIntervalMs}. Input must be greater than zero.");
            }

            _timestamp = timestamp;
            _requestLimit = requestLimit;
            _requestIntervalTicks = requestIntervalMs * TimeSpan.TicksPerMillisecond;
        }

        public bool RequestConforms()
        {
            var requestConforms = false;

            lock (_syncObject)
            {
                var currentTime = _timestamp.GetTimestamp();
                var elapsedTime = _windowStartTime.HasValue ? (currentTime - _windowStartTime.Value) : 0;

                // Window was started previously
                if (_windowStartTime.HasValue)
                {
                    // We have reached the end of the current window
                    if (elapsedTime >= _requestIntervalTicks)
                    {
                        // We exceeded the current window by two or more window lengths
                        // We can treat this as though it is the first request as the previous window should have no bearing on incoming requests
                        if (elapsedTime >= _requestIntervalTicks * 2)
                        {
                            _windowStartTime = currentTime;
                            _prevRequestCount = 0;
                            _requestCount = 0;

                            elapsedTime = 0;
                        }
                        // We exceeded or met the current window's end time since our last request
                        // The current will become the previous and we can calculate our location within the new current
                        else
                        {
                            _windowStartTime = _windowStartTime + _requestIntervalTicks;
                            _prevRequestCount = _requestCount;
                            _requestCount = 0;

                            elapsedTime = currentTime - _windowStartTime.Value;
                        }
                    }
                }
                // This is the first request, start the window
                else
                {
                    _windowStartTime = currentTime;
                }

                var weightedRequestCount = _prevRequestCount * ((double)(_requestIntervalTicks - elapsedTime) / _requestIntervalTicks) + _requestCount + 1;
                if (weightedRequestCount <= _requestLimit)
                {
                    _requestCount++;
                    requestConforms = true;
                }
            }

            return requestConforms;
        }
    }
}
