using System.Collections.Generic;

namespace RateLimiter.Model.Rule.Config
{
    public abstract class RuleConfigIpBase : RuleConfigBase
    {
        public List<string> IpAddresses { get; set; } = new List<string>();

        public RuleConfigIpBase(EnumRateLimitRule rule) : base(rule)
        {
        }
    }
}
