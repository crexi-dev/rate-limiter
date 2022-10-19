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

        public bool ProcessRules(ClientRequest ClientRequest)
        {
            foreach (var rule in rules)
            {
                if (rule.IsEnabled(ClientRequest) && !rule.Validate(ClientRequest))
                    return false;
            }
            return true;
        }
    }
}