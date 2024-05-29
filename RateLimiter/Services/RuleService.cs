using System;
using System.Collections.Generic;
using RateLimiter.Interfaces;

namespace RateLimiter.Services
{
    public class RuleService
    {
        private readonly Dictionary<string, Dictionary<string, List<IRule>>> _rules = new();
        private string _currentResource;
        private string _currentRegion;

        public RuleService ConfigureResource(string resource)
        {
            _currentResource = resource;
            _rules[resource] = new Dictionary<string, List<IRule>>();
            return this;
        }

        public RuleService ForRegion(string region)
        {
            _currentRegion = region;
            if (!_rules[_currentResource].ContainsKey(region))
            {
                _rules[_currentResource][region] = new List<IRule>();
            }
            return this;
        }

        public RuleService AddRule(IRule rule)
        {
            _rules[_currentResource][_currentRegion].Add(rule);
            return this;
        }

        public IRule[] GetRulesForResource(string resource, string token)
        {
            string regionPrefix = token.Split('-')[0];
            if (_rules.TryGetValue(resource, out var regions))
            {
                return regions[regionPrefix].ToArray();
            }

            return Array.Empty<IRule>();
        }
    }

}
