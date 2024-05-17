using RateLimiter.Rules;
using System;

namespace RateLimiter.RuleTemplates;

public interface IRuleTemplate
{
    Type GetParamsType();
    Type GetRuleConstructorType();
}