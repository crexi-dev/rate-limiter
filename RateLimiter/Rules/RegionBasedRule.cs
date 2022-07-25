using RuleLimiterTask.Cache;

namespace RuleLimiterTask.Rules
{
    public abstract class RegionBasedRule : IRule
    {
        private readonly Region _region;
        protected IRule _innerRule;

        public RegionBasedRule(Region region)
        {
            _region = region;
        }

        public abstract bool IsValid(UserRequest request, CacheEntry cacheEntry);
    }
}
