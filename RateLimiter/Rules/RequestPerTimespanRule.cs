using System;
using RateLimiter.Cache;
using RateLimiter.Rules.Settings;

namespace RateLimiter.Rules
{
    public class RequestPerTimespanRule : IRule
    {
        private readonly RequestPerTimespanSettings _settings;

        public RequestPerTimespanRule(RequestPerTimespanSettings settings)
        {
            _settings = settings;
        }

        public bool IsValid(UserRequest request, CacheEntry cacheEntry)
        {
            var timestampInMs = TimeSpan.FromMilliseconds(_settings.TimespanInMs);

            if (request.RequestTime - cacheEntry.GetFirstEntry() > timestampInMs ||
                cacheEntry.GetCount() < _settings.Count) 
            {
                if(cacheEntry.GetCount() == _settings.Count)
                {
                    cacheEntry.RemoveFirst();
                }

                cacheEntry.Add(request.RequestTime);

                return true;
            }

            return false;
        }
    }
}
