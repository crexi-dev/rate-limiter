using System.Collections.Generic;
using RateLimiter.RulesEngine.Repository;

namespace RateLimiter.RulesEngine.Client
{
    public class RulesEngineProxy : IRulesEngineProxy {
        public IEnumerable<Rule> GetRules(string serverIP) {
            var ruleRepository = new RuleRepository();
            var rulesEngine = new RulesEngine(ruleRepository);

            return rulesEngine.GetRules(serverIP);
        }
    }
}