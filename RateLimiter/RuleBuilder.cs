using RateLimiter.Repositories;
using RateLimiter.Rules;
using RateLimiter.RuleTemplates;
using System;

namespace RateLimiter;

public class RuleBuilder
{
    private readonly IRuleTemplateDetector _ruleTemplateDetector;
    private readonly IRuleRepository _ruleRepository;

    public RuleBuilder(IRuleTemplateDetector ruleTemplateDetector, IRuleRepository ruleRepository)
    {
        _ruleTemplateDetector = ruleTemplateDetector;
        _ruleRepository = ruleRepository;
    }
    public RuleTemplateCollection GetTemplates()
    {
        var ruleTemplates = _ruleTemplateDetector.FindTemplates();
        return new RuleTemplateCollection(ruleTemplates);
    }

    // Change to result
    public void Add(string resource, Guid clientId, IRuleTemplate ruleTemplate, RuleTemplateParams ruleParams)
    {
        _ruleRepository.Save(resource, clientId, ruleTemplate, ruleParams);
    }
}
