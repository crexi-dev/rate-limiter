using RateLimiter.Rules;
using System.Collections.Generic;

namespace RateLimiter
{
    public class RateLimiterValidator
    {
        private readonly Dictionary<string, List<IRule>> _resourceRules = new Dictionary<string, List<IRule>>();

        public void AddRule(string resource, IRule rule)
        {
            if (!_resourceRules.TryGetValue(resource, out var rules))
            {
                rules = new List<IRule>();
                _resourceRules[resource] = rules;
            }

            rules.Add(rule);
        }

        public bool IsRequestAllowed(string resource, string client)
        {
            if (!_resourceRules.TryGetValue(resource, out var rules))
            {
                // If there are no rules for the resource, allow the request
                return true;
            }

            foreach (var rule in rules)
            {
                if (!rule.IsRequestAllowed(resource, client))
                {
                    return false;
                }
            }

            return true;
        }
    }
}