using System;
using RateLimiter.Rules.Constructors;
using RateLimiter.RuleTemplates.Params;
using RateLimiter.RuleTemplates.RequestConverters;

namespace RateLimiter.RuleTemplates;

public class RequestByTimeSpanRuleTemplate : IRuleTemplate
{
    public Type GetParamsType()
    {
        return typeof(RequestByTimeSpanRuleTemplateParams);
    }

    public Type GetRequestConverterType()
    {
        return typeof(RequestByTimeSpanRuleRequestConverter);
    }

    public Type GetRuleConstructorType()
    {
        return typeof(RequestByTimeSpanRuleConstructor);
    }
}
