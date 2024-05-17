using System;

namespace RateLimiter.RuleTemplates;

public interface IRuleTemplate
{
    Type GetParamsType();
}