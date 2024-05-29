using System;
using System.Collections.Generic;
using RateLimiter.Interfaces;

namespace RateLimiter.Services
{
    public class RuleProvider
    {
        private readonly Dictionary<Tuple<string, string>, List<IRule>> _rules = new();
        public readonly IDateTimeWrapper _dateTimeWrapper;

        public RuleProvider(IDateTimeWrapper dateTimeWrapper)
        {
            _dateTimeWrapper = dateTimeWrapper;
        }

        public RuleProvider AddRule(string resource, string region, IRule rule)
        {
            var key = Tuple.Create(resource, region);
            if (!_rules.ContainsKey(key))
            {
                _rules[key] = new List<IRule>();
            }
            _rules[key].Add(rule);
            return this;
        }

        public List<IRule> GetRulesForResource(string resource, string region)
        {
            var key = Tuple.Create(resource, region);
            return _rules.ContainsKey(key) ? _rules[key] : new List<IRule>();
        }
    }
}
