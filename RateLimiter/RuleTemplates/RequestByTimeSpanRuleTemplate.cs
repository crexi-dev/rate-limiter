using System;
using RateLimiter.RuleTemplates.Params;

namespace RateLimiter.RuleTemplates;

public class RequestByTimeSpanRuleTemplate : IRuleTemplate
{
    public Type GetParamsType()
    {
        return typeof(RequestByTimeSpanRuleTemplateParams);
    }

    public Type GetRuleConstructorType()
    {
        return typeof(RequestByTimeSpanRuleConstructor);
    }
}
