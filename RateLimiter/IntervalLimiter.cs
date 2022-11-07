using RateLimiter.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter
{
    /// <summary>
    /// Request limiter based on number of requests per timespan
    /// </summary>
    public class IntervalLimiter : ILimiter
    {
        private readonly TimeSpan _interval;
        private ConcurrentDictionary<string, DateTime> _histories;

        public IntervalLimiter(TimeSpan interval)
        {
            _interval = interval;
            _histories = new ConcurrentDictionary<string, DateTime>();
        }

        public bool Validate(string requestToken)
        {
            var now = DateTime.UtcNow;
            var history = _histories.GetOrAdd(requestToken, DateTime.MinValue);
            _histories[requestToken] = now;
            return (now - history) >= _interval;
        }
    }
}
