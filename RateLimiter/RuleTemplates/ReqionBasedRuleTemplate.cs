using System;
using RateLimiter.RuleTemplates.Params;

namespace RateLimiter.RuleTemplates
{
    internal class ReqionBasedRuleTemplate : IRuleTemplate
    {
        public Type GetParamsType()
        {
            return typeof(ReqionBasedRuleTemplateParams);
        }
    }
}