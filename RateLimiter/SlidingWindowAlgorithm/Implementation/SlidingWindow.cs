using RateLimiter.SlidingWindowAlgorithm.TimeStampUtilities;
using System;

namespace RateLimiter.SlidingWindowAlgorithm.Implementation
{
    public class SlidingWindow
    {
        public ITimestamp Timestamp;

        private readonly object _syncObject = new object();

        private readonly long _requestIntervalTicksCount;
        private readonly int _requestLimitValue;

        private long? _windowStartTime;
        private int _previousRequestCount;
        private int _requestCount;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timestamp">Current timestamp</param>
        /// <param name="requestLimit">Limited number of requests</param>
        /// <param name="requestIntervalMilliseconds">Request limit time in milliseconds</param>
        public SlidingWindow(ITimestamp timestamp, int requestLimit, int requestIntervalMilliseconds)
        {
            if (requestLimit < 1)
            {
                throw new ArgumentException($"Invalid value ({requestLimit}) has been received for {nameof(requestLimit)}. The value must be greater than or equal to 1.");
            }

            if (requestIntervalMilliseconds <= 0)
            {
                throw new ArgumentException($"Invalid value ({requestIntervalMilliseconds}) has been received for {nameof(requestIntervalMilliseconds)}. Input must be greater than 0.");
            }

            Timestamp = timestamp;
            _requestLimitValue = requestLimit;
            _requestIntervalTicksCount = requestIntervalMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Checks, if request can be processed with sliding window timeline
        /// </summary>
        /// <returns></returns>
        public bool RequestConforms()
        {
            var requestConforms = false;

            lock (_syncObject)
            {
                var currentTime = Timestamp.GetTimestamp();
                var elapsedTime = _windowStartTime.HasValue ? (currentTime - _windowStartTime.Value) : 0;

                // Case, when the window was started before
                if (_windowStartTime.HasValue)
                {
                    // End of the current window
                    if (elapsedTime >= _requestIntervalTicksCount)
                    {
                        // When exceeded the current window by 2 or more window lengths
                        // treating as though it is the first request, as the previous window should have no bearing on incoming requests
                        if (elapsedTime >= _requestIntervalTicksCount * 2)
                        {
                            _windowStartTime = currentTime;
                            _previousRequestCount = 0;
                            _requestCount = 0;

                            elapsedTime = 0;
                        }
                        // Otherwise exceeded or met the current window's end time since our last request
                        // So the current window will become the previous one and we can calculate our location within the new current
                        else
                        {
                            _windowStartTime = _windowStartTime + _requestIntervalTicksCount;
                            _previousRequestCount = _requestCount;
                            _requestCount = 0;

                            elapsedTime = currentTime - _windowStartTime.Value;
                        }
                    }
                }
                // Initial request, starting window
                else
                {
                    _windowStartTime = currentTime;
                }

                // Calculating and comparing
                var weightedRequestCount = _previousRequestCount * ((double)(_requestIntervalTicksCount - elapsedTime) / _requestIntervalTicksCount) + _requestCount + 1;
                if (weightedRequestCount <= _requestLimitValue)
                {
                    _requestCount++;
                    requestConforms = true;
                }
            }

            return requestConforms;
        }

        /// <summary>
        /// Creates a shallow copy
        /// </summary>
        /// <returns>Copied new object</returns>
        public SlidingWindow ShallowCopy()
        {
            return (SlidingWindow)this.MemberwiseClone();
        }
    }
}
