using System.Collections.Generic;
using System;
using RateLimiter.Storage;
using System.Linq;

namespace RateLimiter.Rules
{
    /// <summary>
    /// This rule is used to restrict number of requests during the defined timespan.
    /// </summary>
    public class FixedRequestNumberRule : IRateLimitRule
    {
        private readonly TimeSpan _timeSpan;
        private readonly long _maxRequestCount;

        public FixedRequestNumberRule(TimeSpan timeSpan, long maxRequestCount)
        {
            _timeSpan = timeSpan;
            if (maxRequestCount <= 0)
                throw new ArgumentOutOfRangeException("Max request count should not be less than 1");

            _maxRequestCount = maxRequestCount;
        }

        public bool IsRequestAllowed(string resource, string token)
        {
            var dateNow = DateTime.UtcNow;

            var key = $"{resource}:{token}";
            bool newKeyAdded = DataStorage.Requests.TryAdd(key, new List<DateTime> { dateNow });
            if (newKeyAdded)
            {
                return true;
            }

            var requests = DataStorage.Requests[key];
            var requestsInLatestRange = requests.Count(req => req >= dateNow - _timeSpan);
            if (requestsInLatestRange < _maxRequestCount)
            {
                requests.Add(dateNow);
                return true;
            }

            return false;
        }
    }
}
