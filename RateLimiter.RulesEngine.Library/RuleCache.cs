using System.Collections.Generic;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Library
{
    public class RuleCache : IRuleCache
    {
        private Dictionary<string, Rule> activeRules;

        public RuleCache()
        {
            this.activeRules = new Dictionary<string, Rule>();
        }

        public Rule this[string key]
        {
            get
            {
                Rule rule;
                this.activeRules.TryGetValue(key, out rule);

                return rule;
            }

            set
            {
                this.activeRules[key] = value;
            }
        }
    }
}