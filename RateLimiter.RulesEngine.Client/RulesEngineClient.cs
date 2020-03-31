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

        public int AddRule(Rule rule)
        {
            return 1;
        }

        public Rule GetRule(string resource, string serverIP) {
            return this.rulesEngineProxy.GetRule(resource, serverIP);
        }

        public bool EvaluateResourceRule()
        {
            return false;
        }

        public bool EvaluateIPRule()
        {
            return false;
        }
    }
}