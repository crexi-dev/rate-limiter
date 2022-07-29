using RateLimiter.Cache;
using RateLimiter.Rules.Settings;

namespace RateLimiter.Rules
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
