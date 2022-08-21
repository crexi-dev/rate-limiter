using System.Collections.Generic;
using RateLimiter.RuleEngines;
using RateLimiter.Rules;

namespace RateLimiter.Configurators
{
    // The purpose of this class is to mimic IOC container
    public sealed class RuleEngine1Configurator
    {
        private static readonly RuleEngine? Instance = null;

        public static RuleEngine GetRuleEngine(List<IRule> rules) =>
            Instance ?? RuleEngine.ConfigureRules(rules);
    }
}