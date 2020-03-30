using System.Collections.Generic;
using RateLimiter.RulesEngine.Repository;

namespace RateLimiter.RulesEngine
{
    internal class RulesEngine : IRulesEngine {
        private IRuleRepository ruleRepository;

        public RulesEngine(IRuleRepository ruleRepository) {
            this.ruleRepository = ruleRepository;
        }

        public int Create(Rule rule) {
            return 1;
        }

        public IEnumerable<Rule> GetRules(string serverIP) {
            // process server IP

            return this.ruleRepository.GetRules(serverIP);
        }
    }
}