using RateLimiter.Rules;
using RateLimiter.Rules.Constructors;
using RateLimiter.RuleTemplates;
using System;

namespace RateLimiter.Repositories;

internal class RuleFactory : IRuleFactory
{
    private IRuleConstructorDetector _ruleConstructorDetector;

    public RuleFactory(IRuleConstructorDetector ruleConstructorDetector)
    {
        _ruleConstructorDetector = ruleConstructorDetector;
    }
    public IRule Create(RuleValue value)
    {
        IRuleTemplate template = value.Template;
        var constructorType = template.GetRuleConstructorType();
        var ruleConstructor = _ruleConstructorDetector.GetConstructor(constructorType);
        return ruleConstructor.Construct(value.Params);
    }
}

public interface IRuleConstructorDetector
{
    IRuleConstructor GetConstructor(Type constructorType);
}