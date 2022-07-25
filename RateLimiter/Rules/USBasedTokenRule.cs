using RuleLimiterTask.Cache;
using RuleLimiterTask.Rules.Settings;

namespace RuleLimiterTask.Rules
{
    public class USBasedTokenRule : RegionBasedRule
    {
        public USBasedTokenRule(RequestPerTimespanSettings settings) : base(Region.US)
        {
            _innerRule = new RequestPerTimespanRule(settings);
        }

        public override bool IsValid(UserRequest request, CacheEntry cacheEntry)
        {
            return _innerRule.IsValid(request, cacheEntry);
        }
    }
}
