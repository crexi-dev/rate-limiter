namespace RateLimiter.Model.Rule.Config
{
    public class RuleConfigIpBlackList : RuleConfigIpBase
    {
        public RuleConfigIpBlackList() : base(EnumRateLimitRule.IpBlackList)
        {
        }
    }
}
