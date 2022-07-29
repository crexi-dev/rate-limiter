using System;
using RateLimiter.Cache;
using RateLimiter.Rules.Settings;

namespace RateLimiter.Rules
{
    public class TimespanSinceLastCallRule : IRule
    {
        private readonly TimespanSinceLastCallSettings _settings;

        public TimespanSinceLastCallRule(TimespanSinceLastCallSettings settings)
        {
            _settings = settings;
        }

        public bool IsValid(UserRequest request, CacheEntry cacheEntry)
        {
            var last = cacheEntry.Last;

            if (last == default || request.RequestTime - last > TimeSpan.FromMilliseconds(_settings.InMs)) 
            {
                cacheEntry.Last = request.RequestTime;

                return true;
            }

            return false;
        }
    }
}
