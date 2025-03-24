namespace RateLimiter.Model.Rule.Config
{
    public abstract class RuleConfigBase
    {
        public EnumRateLimitRule Rule { get; private set; }

        public RuleConfigBase(EnumRateLimitRule rule)
        {
            Rule = rule;
        }
    }
}
