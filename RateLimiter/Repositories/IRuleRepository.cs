using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.RuleTemplates;
using System;

namespace RateLimiter.Repositories;

public interface IRuleRepository
{
    IRuleCollection GetRules(Token token);
    void Save(string resource, Guid clientId, IRuleTemplate ruleTemplate, RuleTemplateParams ruleParams);
}