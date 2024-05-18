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

        public Type GetRequestConverterType()
        {
            throw new NotImplementedException();
        }

        public Type GetRuleConstructorType()
        {
            throw new NotImplementedException();
        }
    }
}