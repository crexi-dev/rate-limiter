using RateLimiter.Interfaces;
using System.Collections.Generic;

namespace RateLimiter.Classes
{
    // Manager class for rate limiting
    public class RateLimitManager
    {
        private readonly Dictionary<string, List<IRateLimitRule>> _rules = new();

        public void AddRule(string resource, IRateLimitRule rule)
        {
            if (_rules.ContainsKey(resource))
            {
                _rules[resource].Add(rule);
            }
            else
            {
                _rules[resource] = new List<IRateLimitRule> { rule };
            }
        }

        public bool IsRequestAllowed(string token, string resource)
        {
            if (!_rules.ContainsKey(resource)) return true;  // No limit if no rules defined

            foreach (var rule in _rules[resource])
            {
                if (!rule.IsRequestAllowed(token, resource))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
