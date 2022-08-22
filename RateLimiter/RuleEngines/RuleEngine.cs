using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Exceptions;
using RateLimiter.Rules;

namespace RateLimiter.RuleEngines
{
    public class RuleEngine
    {
        private List<IRule> Rules { get; set; } = null!;
        private static RuleEngineModes Mode { get; set; }

        private RuleEngine(List<IRule> rules, RuleEngineModes mode)
        {
            Rules = rules;
            Mode = mode;
        }

        public static RuleEngine ConfigureRules(List<IRule> rules,
            RuleEngineModes mode = RuleEngineModes.MostRestrictive)
        {
            return new(rules, mode);
        }

        public void Run(string token)
        {
            switch (Mode)
            {
                case RuleEngineModes.MostRestrictive:
                    ExecuteMostRestrictive(token);
                    break;

                case RuleEngineModes.LeastRestrictive:
                    ExecuteLeastRestrictive(token);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Mode), Mode, null);
            }
        }

        private void ExecuteMostRestrictive(string token)
        {
            if (Rules.Any(rule => !rule.Execute(token)))
            {
                throw new RequestsOutOfLimitException("Limit Exceeded");
            }
        }

        // TODO: Need to Implement least restrictive algorithm
        private void ExecuteLeastRestrictive(string token)
        {
            if (Rules.All(rule => !rule.Execute(token)))
            {
                throw new RequestsOutOfLimitException("Limit Exceeded");
            }
        }
    }
}