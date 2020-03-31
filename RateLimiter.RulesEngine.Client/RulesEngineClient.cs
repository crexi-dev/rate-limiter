using System.Collections.Generic;
using RateLimiter.RulesEngine.Library;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Client
{
    public class RulesEngineClient : IRulesEngine {
        private IRulesEngineProxy rulesEngineProxy;

        public RulesEngineClient(IRulesEngineProxy rulesEngineProxy) {
            this.rulesEngineProxy = rulesEngineProxy;
        }

        public int Create(Rule rule)
        {
            return 1;
        }

        public IEnumerable<Rule> GetRules(string serverIP) {
            return this.rulesEngineProxy.GetRules(serverIP);
        }
    }
}