using RateLimiter.Rules;

namespace RateLimiter;

public class RuleBuilder
{
    private readonly IRuleTemplateDetector _ruleTemplateDetector;

    public RuleBuilder(IRuleTemplateDetector ruleTemplateDetector)
    {
        _ruleTemplateDetector = ruleTemplateDetector;
    }
    public RuleTemplateCollection GetTemplates()
    {
        var ruleTemplates = _ruleTemplateDetector.FindTemplates();
        return new RuleTemplateCollection(ruleTemplates);
    }

    public void AssingRules(string resource, RuleCollection rules)
    {

    }
}
