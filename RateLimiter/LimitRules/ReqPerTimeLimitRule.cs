using System;
using System.Collections.Generic;

namespace RateLimiter.LimitRules
{
    public class ReqPerTimeLimitRule : ILimitRule
    {
        public TimeSpan RequestsTimeSpan { get; set; } = TimeSpan.FromSeconds(5);
        public int RequestsLimit { get; set; } = 5;

        public string Name { get; } = "RequestsPerTime";

        private readonly Dictionary<string, ReqPerTimeLimitLimitObject> _cache = new();

        public ReqPerTimeLimitRule(TimeSpan requestsTimeSpan, int requestsLimit)
        {
            RequestsTimeSpan = requestsTimeSpan;
            RequestsLimit = requestsLimit;
        }

        public bool Validate(string resource, string identifer)
        {
            var cacheKey = $"{resource}:{identifer}";
            var limitObject = _cache.GetValueOrDefault(cacheKey);

            Console.WriteLine($"{nameof(ReqPerTimeLimitRule)}: {cacheKey} - {limitObject?.Count}");

            // Need to clear out old values, ideally we would use a caching class
            // instead of a dictionary which would handle this for us async.
            foreach (KeyValuePair<string, ReqPerTimeLimitLimitObject> value in _cache)
            {
                if ((DateTime.UtcNow - value.Value.StartWindow) > RequestsTimeSpan)
                {
                    _cache.Remove(value.Key);
                }
            }

            // Start cache for new keys.
            if (limitObject == null)
            {
                var newLimitObject = new ReqPerTimeLimitLimitObject(DateTime.UtcNow, 1);
                _cache.Add(cacheKey, newLimitObject);

                Console.WriteLine($"{nameof(ReqPerTimeLimitRule)}: {cacheKey} - New Key");

                return true;
            }

            // Check current count and update.
            if (limitObject.Count < RequestsLimit && (DateTime.UtcNow - limitObject.StartWindow) <= RequestsTimeSpan)
            {
                limitObject.Count++;

                Console.WriteLine($"{nameof(ReqPerTimeLimitRule)}: {cacheKey} - Allowed");

                return true;
            }

            Console.WriteLine($"{nameof(ReqPerTimeLimitRule)}: {cacheKey} - Denied");

            return false;
        }
    }

    internal class ReqPerTimeLimitLimitObject
    {
        public DateTime StartWindow { get; set; }
        public int Count { get; set; }

        public ReqPerTimeLimitLimitObject(DateTime startWindow, int count)
        {
            StartWindow = startWindow;
            Count = count;
        }
    }
}
