using RuleLimiterTask.Cache;
using RuleLimiterTask.Rules.Settings;

namespace RuleLimiterTask.Rules
{
    public class TimespanSinceLastCallRule : BaseRule
    {
        private readonly TimespanSinceLastCallSettings _settings;

        public TimespanSinceLastCallRule(TimespanSinceLastCallSettings settings)
        {
            _settings = settings;
        }

        public override bool IsValid(UserRequest request, ICacheService cache)
        {
            var key = GenerateKey(request.Token.UserId);

            var cacheEntry = cache.Get<CacheEntry>(key) ?? new();
            var last = cacheEntry.Last;

            if (last == default || request.RequestTime - last > TimeSpan.FromMilliseconds(_settings.TimespanPassedSinceLastCallInMs)) 
            {
                cacheEntry.Last = request.RequestTime;

                return true;
            }

            return false;
        }
    }
}
