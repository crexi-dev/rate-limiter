using RateLimiter.Repositories;

namespace RateLimiter.RuleTemplates.Params
{
    internal class RegionBasedRuleTemplateParams : RuleTemplateParams
    {
        public string Region { get; set; } = null!;
        public RuleValue InnerRule { get; set; } = null!;
    }
}