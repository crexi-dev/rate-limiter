using System;
using RateLimiter.RuleTemplates.Params;

namespace RateLimiter.RuleTemplates
{
    internal class TimeSpanFromLastCallRuleTemplate : IRuleTemplate
    {
        public Type GetParamsType()
        {
            return typeof(TimeSpanFromLastCallRuleTemplateParams);
        }
    }
}