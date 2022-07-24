using RuleLimiterTask.Rules.Settings;

namespace RuleLimiterTask.Rules
{
    public class USBasedTokenRule : RegionBasedRule
    {
        public USBasedTokenRule(RequestPerTimespanSettings settings) : base(Region.US)
        {
            _innerRule = new RequestPerTimespanRule(settings);
        }

        public override bool IsValid(UserRequest request, ICacheService cache)
        {
            return _innerRule.IsValid(request, cache);
        }
    }
}
