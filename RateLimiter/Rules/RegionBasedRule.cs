namespace RuleLimiterTask.Rules
{
    public abstract class RegionBasedRule : BaseRule
    {
        private readonly Region _region;
        protected IRule _innerRule;

        public RegionBasedRule(Region region)
        {
            _region = region;
        }
    }
}
