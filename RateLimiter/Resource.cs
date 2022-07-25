using RuleLimiterTask.Cache;
using RuleLimiterTask.Rules;

namespace RuleLimiterTask
{
    public class Resource
    {
        private readonly List<IRule> _rules = new();

        public void ApplyRule(IRule rule)
        {
            _rules.Add(rule);
        }

        public bool CheckAccess(UserRequest request, ICacheService cache)
        {
            var validCacheInfo = new Dictionary<IRule, CacheEntry>();

            var key = request.Token.UserId.ToString();

            var cacheEntry = cache.Get<CacheEntry>(key) ?? new();

            foreach (var rule in _rules) 
            {
                if (!rule.IsValid(request, cacheEntry))
                    return false; // this call is not valid, so we return and not even cache it

                validCacheInfo.Add(rule, cacheEntry);
            }

            _rules.ForEach(r => cache.Set(key, validCacheInfo[r]));

            return true;
        }
    }
}
