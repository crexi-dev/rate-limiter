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
    private readonly IRequestConverterFactory _requestConverterFactory;

    public RuleRepository(IRuleFactory ruleFactory, IRequestConverterFactory requestConverterFactory)
    {
        _ruleFactory = ruleFactory;
        _requestConverterFactory = requestConverterFactory;
    }
    
    public IRuleCollection GetRules(string resource, Token token)
    {
        List<ValidateReadyRule> rules = new();
        if(_storage.TryGetValue(new RuleKey(resource, token.ClientId), out RuleValue? value))
        {
            rules.Add(new ValidateReadyRule(_ruleFactory.Create(value), _requestConverterFactory.Create(value), value.Params) );
        }
        return new RuleCollection(rules);
    }

    public void Save(string resource, Guid clientId, IRuleTemplate ruleTemplate, RuleTemplateParams ruleParams)
    {
        RuleValue newValue = new(ruleTemplate, ruleParams);
        _storage.AddOrUpdate(new RuleKey(resource, clientId), newValue, (key, existing) => newValue);
    }
}
