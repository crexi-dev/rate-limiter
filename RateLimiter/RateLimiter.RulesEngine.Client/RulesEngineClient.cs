using System.Collections.Generic;

namespace RateLimiter.RulesEngine
{
    public class RulesEngineClient : IRulesEngineClient {
        private IRulesEngineProxy rulesEngineProxy;

        public RulesEngineClient(IRulesEngineProxy rulesEngineProxy) {
            this.rulesEngineProxy = rulesEngineProxy;
        }

        public IEnumerable<Rule> GetRules(string serverIP) {
            return this.rulesEngineProxy.GetRules(serverIP);
        }
    }
}