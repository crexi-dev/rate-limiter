using RateLimiter.Domain.Resource;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.ApiLimiter
{
    public class TokenBucket : ITokenBucket
    {
        // This list is the set of rules for this resource. The list remains static after initialization.
        private readonly List<IRule> _rules;

        // Token : Rule
        // As we add more tokens, we add them to this dictionary and clone the rule set above.
        private ConcurrentDictionary<string, IEnumerable<IRule>> _rulesPerToken;

        public TokenBucket()
        {
            _rules = new List<IRule>();
            _rulesPerToken = new ConcurrentDictionary<string, IEnumerable<IRule>>();
        }


        public bool Verify(string token)
        {
            var rules = _rulesPerToken.GetOrAdd(token, (t) =>
            {
                List<IRule> rulesPerToken = new List<IRule>();
                foreach (var rule in _rules)
                {
                    rulesPerToken.Add(rule.Clone() as IRule);
                }
                return rulesPerToken.AsEnumerable();
            });

            bool passed = true;
            foreach (var rule in rules)
            {
                // Do not break if a rule does not pass - all rules need to increment their counts
                // regarless if previous rules did not pass
                if (!rule.NewVisitAndRuleCheck())
                    passed = false;
            }
            return passed;
        }

        /// <summary>
        /// Only gets called on Initialization
        /// </summary>
        /// <param name="rules"></param>
        public void AddRules(IEnumerable<IRule> rules)
        {
            _rules.AddRange(rules);
        }
    }
}
