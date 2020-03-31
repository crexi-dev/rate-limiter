using System.Collections.Generic;
using RateLimiter.RulesEngine.Library;
using RateLimiter.RulesEngine.Library.Repository;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Client
{
    public class RulesEngineProxy : IRulesEngine {

        public int Create(Rule rule)
        {
            return 1;
        }

        public IEnumerable<Rule> GetRules(string serverIP) {
            var ruleRepository = new RuleRepository();
            var rulesEngine = new RulesEngine(ruleRepository);

            return rulesEngine.GetRules(serverIP);
        }
    }
}