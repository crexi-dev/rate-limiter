using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Application.Interfaces;

namespace RateLimiter.Rules
{
    public class PeriodAccessRule: AccessRule
    {
        private readonly ICache _cache;
        private readonly TimeSpan _period;
        private readonly int _requestsCount;
        private readonly string _resourceName;
        private readonly string _cacheKeyTemplate;

        public PeriodAccessRule(ICache cache, TimeSpan period, int requestsCount, string resourceName)
        {
            if (requestsCount < 1)
            {
                throw new ArgumentException(nameof(requestsCount));
            }

            if (string.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentException(nameof(resourceName));
            }

            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _period = period;
            _requestsCount = requestsCount;
            _resourceName = resourceName;
            _cacheKeyTemplate = $"{nameof(PeriodAccessRule)}_" + resourceName + "_{0}";
        }

        public override bool Validate(string resourceName, string accessKey)
        {
            if (resourceName != _resourceName)
            {
                return false;
            }

            var key = string.Format(_cacheKeyTemplate, accessKey);
            var offsetNow = DateTimeOffset.Now;
            var beginPeriod = offsetNow.AddSeconds(-_period.TotalSeconds);
            bool passed;

            if (_cache.TryGet(key, out List<DateTimeOffset>? requests))
            {
                requests = requests!.Where(t => t >= beginPeriod).ToList();
                passed = requests.Count < _requestsCount;
                requests.Add(offsetNow);
                _cache.SetWithSlidingExpiration(key, requests, _period);
            }
            else
            {
                var requestsNew = new List<DateTimeOffset> {offsetNow};
                _cache.SetWithSlidingExpiration(key, requestsNew, _period);
                passed = true;
            }

            return passed;
        }
    }
}
