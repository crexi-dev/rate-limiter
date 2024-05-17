using RateLimiter.Repositories;
using RateLimiter.Rules;
using RateLimiter.RuleTemplates.Params;

namespace RateLimiter.RuleTemplates
{
    internal class RegionBasedRuleConstructor : IRuleConstructor
    {
        private IRuleFactory _ruleFactory;

        public RegionBasedRuleConstructor(IRuleFactory ruleFactory)
        {
            _ruleFactory = ruleFactory;
        }
        public IRule Construct(RuleTemplateParams templateParams)
        {
            if(templateParams is RegionBasedRuleTemplateParams validParams)
            {
                return new RegionBasedRule(validParams.Region, _ruleFactory.Create(validParams.InnerRule));
            }
            throw new InvalidTemplateParamsException();
        }
    }
}