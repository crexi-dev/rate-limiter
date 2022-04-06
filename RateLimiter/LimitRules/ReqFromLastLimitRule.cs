using System;
using System.Collections.Generic;

namespace RateLimiter.LimitRules
{
    public class ReqFromLastLimitRule : ILimitRule
    {
        public TimeSpan RequestsTimeSpan { get; set; } = TimeSpan.FromSeconds(5);

        public string Name { get; } = "RequestsFromLast";

        private readonly Dictionary<string, ReqFromLastLimitObject> _cache = new();

        public ReqFromLastLimitRule(TimeSpan requestsTimeSpan)
        {
            RequestsTimeSpan = requestsTimeSpan;
        }

        public bool Validate(string resource, string identifer)
        {
            // Need to clear out old values, ideally we would use a caching class
            // instead of a dictionary which would handle this for us async.
            foreach (KeyValuePair<string, ReqFromLastLimitObject> value in _cache)
            {
                if ((DateTime.UtcNow - value.Value.StartWindow) > RequestsTimeSpan)
                {
                    _cache.Remove(value.Key);
                }
            }

            var cacheKey = $"{resource}:{identifer}";
            var limitObject = _cache.GetValueOrDefault(cacheKey);

            Console.WriteLine($"{nameof(ReqFromLastLimitRule)}: {cacheKey}");

            // Start cache for new keys.
            if (limitObject == null)
            {
                var newLimitObject = new ReqFromLastLimitObject(DateTime.UtcNow);
                _cache.Add(cacheKey, newLimitObject);

                Console.WriteLine($"{nameof(ReqFromLastLimitRule)}: {cacheKey} - Allowed");

                return true;
            }

            // Key was found so time hasn't passed.
            Console.WriteLine($"{nameof(ReqFromLastLimitRule)}: {cacheKey} - Denied");

            return false;
        }
    }

    internal class ReqFromLastLimitObject
    {
        public DateTime StartWindow { get; set; }

        public ReqFromLastLimitObject(DateTime startWindow)
        {
            StartWindow = startWindow;
        }
    }
}
