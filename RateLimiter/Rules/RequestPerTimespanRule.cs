using RuleLimiterTask.Cache;
using RuleLimiterTask.Rules.Settings;

namespace RuleLimiterTask.Rules
{
    public class RequestPerTimespanRule : BaseRule
    {
        private readonly RequestPerTimespanSettings _settings;

        public RequestPerTimespanRule(RequestPerTimespanSettings settings)
        {
            _settings = settings;
        }

        public override bool IsValid(UserRequest request, ICacheService cache)
        {
            var key = GenerateKey(request.Token.UserId);

            var cacheEntry = cache.Get<CacheEntry>(key) ?? new();
            var cacheEntryCount = cacheEntry.GetCount();

            var timestampInMs = TimeSpan.FromMilliseconds(_settings.TimespanInMs);

            var countByTimespan = cacheEntry.GetCountByTimespan(request.RequestTime, timestampInMs);

            if (countByTimespan < _settings.Count)
            {
                var last = cacheEntry.GetLastEntry();
                if (request.RequestTime - last > timestampInMs)
                {
                    cacheEntry.Clear();
                }
                else if (cacheEntryCount == _settings.Count)
                {
                    cacheEntry.RemoveFirst();
                }

                cacheEntry.Add(request.RequestTime);
                cache.Set(key, cacheEntry);

                return true;
            }

            return false;
        }
    }
}
