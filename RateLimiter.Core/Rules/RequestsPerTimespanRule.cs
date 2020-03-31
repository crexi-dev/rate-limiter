using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace RateLimiter.Core.Rules
{
    public class RequestsPerTimespanRule : IRule
    {
        public RequestsPerTimespanRule(string sourceIdentifier, int maxRequestCount, int seconds)
        {
            if (string.IsNullOrEmpty(sourceIdentifier))
            {
                throw new ArgumentException($"{nameof(sourceIdentifier)} must be provided.");
            }

            SourceIdentifier = sourceIdentifier;
            MaxRequestCount = maxRequestCount;
            Seconds = seconds;
        }

        public int MaxRequestCount { get; }

        public int Seconds { get; }

        private static ConcurrentDictionary<string, int> RequestBuckets { get; } = new ConcurrentDictionary<string, int>();

        private static string CacheIdentifier { get; } = nameof(RequestsPerTimespanRule);

        private static Dictionary<string, Timer> RequestTimers { get; } = new Dictionary<string, Timer>();

        private string SourceIdentifier { get; }

        public bool AllowExecution(string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                throw new ArgumentException("Auth token must be provided.");
            }

            var key = $"{CacheIdentifier}:{SourceIdentifier}:{authToken}";

            if (RequestBuckets.TryGetValue(key, out var activeRequests))
            {
                if (activeRequests < MaxRequestCount)
                {
                    IncrementRequestBucket(key, activeRequests);
                    return true;
                }

                return false;
            }

            IncrementRequestBucket(key, 0);

            return true;
        }

        public string GetNotAllowedReason(string authToken)
        {
            return $"You have exceeded the max allowance of {MaxRequestCount} request(s) per {Seconds} second(s). Please try again later.";
        }

        private void IncrementRequestBucket(string key, int activeRequests)
        {
            RequestBuckets[key] = activeRequests + 1;

            var timerKey = Guid.NewGuid().ToString();

            var timer = new Timer(state =>
            {
                    if (RequestBuckets[key] <= 1)
                    {
                        RequestBuckets.TryRemove(key, out var value);
                    }
                    else
                    {
                        RequestBuckets[key]--;
                    }

                    RequestTimers.Remove(timerKey);
            }, null, TimeSpan.FromSeconds(Seconds), TimeSpan.FromMilliseconds(-1));

            RequestTimers.Add(timerKey, timer);
        }
    }
}