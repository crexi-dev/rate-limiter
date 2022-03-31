using System;
using RateLimiter.Application.Interfaces;

namespace RateLimiter.Rules
{
    public class IntervalAccessRule: AccessRule
    {
        private readonly ICache _cache;
        private readonly TimeSpan _interval;
        private readonly string _resourceName;
        private readonly string _cacheKeyTemplate;

        public IntervalAccessRule(ICache cache, TimeSpan interval, string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentException(nameof(resourceName));
            }

            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _interval = interval;
            _resourceName = resourceName;
            _cacheKeyTemplate = $"{nameof(IntervalAccessRule)}_" + resourceName + "_{0}";
        }

        public override bool Validate(string resourceName, string accessKey)
        {
            if (resourceName != _resourceName)
            {
                return false;
            }

            var cacheKey = string.Format(_cacheKeyTemplate, accessKey);
            var offsetNow = DateTimeOffset.Now;
            bool passed;

            if (_cache.TryGet(cacheKey, out DateTimeOffset previousRequestOffset))
            {
                passed = offsetNow - previousRequestOffset > _interval;
                _cache.SetWithAbsoluteExpiration(cacheKey, offsetNow, _interval);
            }
            else
            {
                passed = true;
                _cache.SetWithAbsoluteExpiration(cacheKey, offsetNow, _interval);
            }

            return passed;
        }
    }
}
