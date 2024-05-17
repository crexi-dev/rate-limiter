using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.RuleTemplates;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RateLimiter.Repositories;

internal class RuleRepository : IRuleRepository
{
    private readonly ConcurrentDictionary<RuleKey, RuleValue> _storage = new ConcurrentDictionary<RuleKey, RuleValue>();
    private IRuleFactory _ruleFactory;

    public RuleRepository(IRuleFactory ruleFactory)
    {
        _ruleFactory = ruleFactory;
    }
    
    public IRuleCollection GetRules(string resource, Token token)
    {
        List<IRule> rules = new();
        if(_storage.TryGetValue(new RuleKey(resource, token.ClientId), out RuleValue? value))
        {
            rules.Add(_ruleFactory.Create(value));
        }
        return new RuleCollection(rules);
    }

    public void Save(string resource, Guid clientId, IRuleTemplate ruleTemplate, RuleTemplateParams ruleParams)
    {
        RuleValue newValue = new(ruleTemplate, ruleParams);
        _storage.AddOrUpdate(new RuleKey(resource, clientId), newValue, (key, existing) => newValue);
    }
}
