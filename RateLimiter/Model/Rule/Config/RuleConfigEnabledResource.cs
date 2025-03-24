namespace RateLimiter.Model.Rule.Config
{
    public class RuleConfigEnabledResource : RuleConfigBase
    {
        public bool Enabled { get; set; } = true;

        public RuleConfigEnabledResource() : base(EnumRateLimitRule.EnabledResource)
        {
        }
    }
}
