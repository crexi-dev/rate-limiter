using RateLimiter.Exceptions;
using RateLimiter.Repositories;
using RateLimiter.RuleTemplates;
using RateLimiter.RuleTemplates.Params;

namespace RateLimiter.Rules.Constructors
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
            if (templateParams is RegionBasedRuleTemplateParams validParams)
            {
                return new RegionBasedRule(validParams.Region, _ruleFactory.Create(validParams.InnerRule));
            }
            throw new InvalidTemplateParamsException();
        }
    }
}