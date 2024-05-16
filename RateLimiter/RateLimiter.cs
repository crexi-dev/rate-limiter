using RateLimiter.Rules;
using System.Collections.Generic;

namespace RateLimiter
{
    public class RateLimiter
    {
        private readonly IDictionary<string, ICollection<IRateLimitRule>> _rules;

        public RateLimiter() : this(new Dictionary<string, ICollection<IRateLimitRule>>())
        {

        }

        public RateLimiter(IDictionary<string, ICollection<IRateLimitRule>> rules)
        {
            _rules = rules;
        }

        public void AddRule(string resource, IRateLimitRule rule)
        {
            if (_rules.TryGetValue(resource, out var rules))
            {
                rules.Add(rule);
            }
            else
            {
                _rules.Add(resource, [rule]);
            }
        }

        public bool IsRequestAllowed(string resource, string token)
        {
            if (!_rules.ContainsKey(resource))
            {
                return true;
            }

            foreach (var rule in _rules[resource])
            {
                if (!rule.IsRequestAllowed(resource, token))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
