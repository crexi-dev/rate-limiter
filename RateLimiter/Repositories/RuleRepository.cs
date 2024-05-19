using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.Rules.RequestConverters;
using RateLimiter.RuleTemplates;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RateLimiter.Repositories;

internal class RuleRepository : IRuleRepository
{
    private readonly ConcurrentDictionary<RuleKey, List<RuleValue>> _storage = new ConcurrentDictionary<RuleKey, List<RuleValue>>();
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
        if(_storage.TryGetValue(new RuleKey(resource, token.ClientId), out List<RuleValue>? values))
        {
            foreach (var value in values) 
            {
                rules.Add(new ValidateReadyRule(_ruleFactory.Create(value), _requestConverterFactory.Create(value), value.Params));
            }
            
        }
        return new RuleCollection(rules);
    }

    public void Save(string resource, Guid clientId, IRuleTemplate ruleTemplate, RuleTemplateParams ruleParams)
    {
        RuleValue newValue = new(ruleTemplate, ruleParams);
        var key = new RuleKey(resource, clientId);
        lock (_storage) { }
        if(!_storage.TryGetValue(key, out var ruleList))
        {
            ruleList = new List<RuleValue>();
        }
        ruleList.Add(newValue);
        _storage.AddOrUpdate(new RuleKey(resource, clientId), ruleList, (key, existing) => { existing.Add(newValue); return existing; });
    }
}
