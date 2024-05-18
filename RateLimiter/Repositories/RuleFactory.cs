using RateLimiter.Repositories.Detectors;
using RateLimiter.Rules;
using RateLimiter.RuleTemplates;

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
