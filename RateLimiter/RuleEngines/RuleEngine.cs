using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Rules;

namespace RateLimiter.RuleEngines
{
    public class RuleEngine
    {
        private static List<IRule> Rules { get; set; } = null!;
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

        // TODO: Need to Implement most restrictive algorithm
        private static void ExecuteMostRestrictive(string token)
        {
            if (Rules.Any(rule => !rule.Execute(token)))
            {
                throw new Exception("Limit Exceeded");
            }
        }

        // TODO: Need to Implement least restrictive algorithm
        private static void ExecuteLeastRestrictive(string token)
        {
            foreach (var rule in Rules)
            {
                rule.Execute(token);
            }
        }
    }
}