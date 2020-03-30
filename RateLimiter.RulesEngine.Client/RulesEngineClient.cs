using System.Collections.Generic;
using RateLimiter.RulesEngine;

namespace RateLimiter.RulesEngine.Client
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