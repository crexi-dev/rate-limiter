using System;

namespace RateLimiter.Model.Rule.Config
{
    public class RuleConfigTimespanThreshold : RuleConfigBase
    {
        public TimeSpan LastCallThreshold { get; set; }
        public int MaxThreshold { get; set; }

        public RuleConfigTimespanThreshold() 
            : base(EnumRateLimitRule.TimespanThreshold) 
        {
        }
    }
}
