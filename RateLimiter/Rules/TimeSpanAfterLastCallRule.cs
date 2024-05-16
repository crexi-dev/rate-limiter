using RateLimiter.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rules
{
    /// <summary>
    /// This rule assure that a certain timespan passed since the last call for the resource.
    /// </summary>
    public class TimeSpanAfterLastCallRule : IRateLimitRule
    {
        private readonly TimeSpan _maxTimeSpanBetweenCalls;

        public TimeSpanAfterLastCallRule(TimeSpan maxTimeSpanBetweenCalls)
        {
            _maxTimeSpanBetweenCalls = maxTimeSpanBetweenCalls;
        }

        public bool IsRequestAllowed(string resource, string token)
        {
            var dateNow = DateTime.UtcNow;
            var key = $"{resource}:{token}";

            var newKeyAdded = DataStorage.Requests.TryAdd(key, new List<DateTime> { DateTime.UtcNow });
            if (newKeyAdded)
            {
                return true;
            }

            var lastRequestTime = DataStorage.Requests[key].Max();
            if (dateNow - lastRequestTime >= _maxTimeSpanBetweenCalls)
            {
                DataStorage.Requests[key].Add(dateNow);
                return true;
            }

            return false;
        }
    }
}
