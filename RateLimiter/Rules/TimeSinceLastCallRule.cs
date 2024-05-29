using System;
using System.Collections.Generic;
using RateLimiter.Interfaces;
using RateLimiter.Models;

namespace RateLimiter.Rules
{
    public class TimeSinceLastCallRule : IRule
    {
        private readonly TimeSpan _timeSpan;
        private readonly Dictionary<string, DateTime> _tokenLastRequestTimes;
        private readonly IDateTimeWrapper _dateTimeWrapper;

        public TimeSinceLastCallRule(TimeSpan timeSpan, IDateTimeWrapper dateTimeWrapper)
        {
            _timeSpan = timeSpan;
            _tokenLastRequestTimes = new Dictionary<string, DateTime>();
            _dateTimeWrapper = dateTimeWrapper;
        }

        public RuleCheckResult CheckRule(string token)
        {
            lock (_tokenLastRequestTimes)
            {
                if (!_tokenLastRequestTimes.TryGetValue(token, out var lastRequestTime))
                {
                    lastRequestTime = DateTime.MinValue;
                }

                var now = _dateTimeWrapper.UtcNow;
                if (now - lastRequestTime > _timeSpan)
                {
                    _tokenLastRequestTimes[token] = now;
                    return new RuleCheckResult(true);
                }

                return new RuleCheckResult(false,
                    "Too many requests. Please try again later.",
                    "TimeSinceLastCallExceeded");
            }
        }
    }
}
