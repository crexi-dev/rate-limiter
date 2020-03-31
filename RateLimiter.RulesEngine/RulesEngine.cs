using System.Collections.Generic;
using RateLimiter.RulesEngine.Library;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine
{
    public class RulesEngine : IRulesEngine {
        private IRuleRepository ruleRepository;
        private IRulesEvaluator rulesEvaluator;

        public RulesEngine(IRuleRepository ruleRepository, IRulesEvaluator rulesEvaluator) {
            this.ruleRepository = ruleRepository;
            this.rulesEvaluator = rulesEvaluator;
        }

        public int AddRule(Rule rule) {
            this.ruleRepository.AddRule(rule);
            return 1;
        }

        public Rule GetRule(string resource, string serverIP) {
            // process server IP

            return this.ruleRepository.GetRule(resource, serverIP);
        }

        public bool EvaluateRules(string resourceName, string ipAddress)
        {
            // evaluate resource rules
            this.rulesEvaluator.EvaluateResourceRule(resourceName);

            // evaluate IP rules
            return false;
        }
    }
}