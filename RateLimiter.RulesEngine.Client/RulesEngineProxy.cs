using System.Collections.Generic;
using RateLimiter.RulesEngine.Library;
using RateLimiter.RulesEngine.Library.Repository;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Client
{
    public class RulesEngineProxy : IRulesEngine {
        private RulesEngine rulesEngine;

        public RulesEngineProxy()
        {
            var rulesRepository = new RuleRepository();
        }

        public int AddRule(Rule rule)
        {
            return 1;
        }

        public Rule GetRule(string resource, string serverIP) {
            return rulesEngine.GetRule(resource, serverIP);
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