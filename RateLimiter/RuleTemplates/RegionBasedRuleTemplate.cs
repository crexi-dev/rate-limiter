using System;
using RateLimiter.Rules;
using RateLimiter.Rules.Constructors;
using RateLimiter.RuleTemplates.Params;

namespace RateLimiter.RuleTemplates
{
    internal class RegionBasedRuleTemplate : IRuleTemplate
    {
        
        public Type GetParamsType()
        {
            return typeof(RegionBasedRuleTemplateParams);
        }

        public Type GetRuleConstructorType()
        {
            return typeof(RegionBasedRuleConstructor);
        }
    }
}