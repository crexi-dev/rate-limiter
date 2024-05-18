using RateLimiter.Rules.Constructors;
using System;

namespace RateLimiter.Repositories.Detectors;

public class RuleConstructorDetector : IRuleConstructorDetector
{
    public IRuleConstructor GetConstructor(Type constructorType)
    {
        var constructor = constructorType.GetConstructor(Array.Empty<Type>());
        if(constructor == null)
        {
            throw new DefaultConstructorExpectedException();
        }
        var ruleConstructor = constructor.Invoke(null);
        if (ruleConstructor is not IRuleConstructor)
        {
            throw new InvalidRuleConstructorType();
        }

        return (IRuleConstructor)ruleConstructor;
    }
}