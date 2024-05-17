using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.RuleTemplates;
using System;
using System.Collections.Concurrent;

namespace RateLimiter.Repositories;

internal class RuleRepository : IRuleRepository
{
    private static readonly ConcurrentDictionary<RuleKey, RuleValue> _storage = new ConcurrentDictionary<RuleKey, RuleValue>();
    public IRuleCollection GetRules(Token token)
    {
        throw new NotImplementedException();
    }

    public void Save(string resource, Guid clientId, IRuleTemplate ruleTemplate, RuleTemplateParams ruleParams)
    {
        RuleValue newValue = new(ruleTemplate, ruleParams);
        _storage.AddOrUpdate(new RuleKey(resource, clientId), newValue, (key, existing) => newValue);
    }
}
