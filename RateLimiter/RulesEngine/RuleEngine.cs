using RateLimiter.Model;
using RateLimiter.RulesEngine.Interfaces;
using System.Collections.Generic;

namespace RateLimiter.RulesEngine
{
    internal class RuleEngine : IRuleEngine
    {
        private readonly IEnumerable<IRateLimiterRule> rules;

        public RuleEngine(IEnumerable<IRateLimiterRule> rules)
        {
            this.rules = rules;
        }

        public bool ProcessRules(UserRequest userRequest)
        {
            foreach (var rule in rules)
            {
                if (rule.IsEnabled(userRequest) && !rule.Validate(userRequest))
                    return false;
            }
            return true;
        }
    }
}
