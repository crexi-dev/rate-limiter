using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Helpers
{
    public static class LimitConfigurator
    {
        private static Dictionary<string, List<Rule>> _rules = new ();

        public static IReadOnlyList<Rule> GetRules(string key) 
            => _rules.GetValueOrDefault(key, new List<Rule>()).AsReadOnly();

        public static void AddRule(string resourceName, Func<RuleBuilder, Rule> builder)
        {
            var newRule = builder.Invoke(new RuleBuilder());

            if (_rules.ContainsKey(resourceName))
            {
                var rule = _rules[resourceName];
                if (rule == null)
                    rule= new List<Rule> ();
                rule.Add(newRule);
            }
            else
                _rules.Add(resourceName, new List<Rule> { newRule });

        }
    }
}
