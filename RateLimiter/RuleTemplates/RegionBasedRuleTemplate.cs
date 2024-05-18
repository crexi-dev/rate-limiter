using System;
using RateLimiter.Rules;
using RateLimiter.Rules.Constructors;
using RateLimiter.RuleTemplates.Params;
using RateLimiter.RuleTemplates.RequestConverters;

namespace RateLimiter.RuleTemplates
{
    internal class RegionBasedRuleTemplate : IRuleTemplate
    {
        
        public Type GetParamsType()
        {
            return typeof(RegionBasedRuleTemplateParams);
        }

        public Type GetRequestConverterType()
        {
            return typeof(RegionBasedRequestConverter);
        }

        public Type GetRuleConstructorType()
        {
            return typeof(RegionBasedRuleConstructor);
        }
    }
}