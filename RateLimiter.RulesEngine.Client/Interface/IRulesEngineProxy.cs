using System.Collections.Generic;
using RateLimiter.RulesEngine.Repository;

namespace RateLimiter.RulesEngine.Client
{
    public interface IRulesEngineProxy {
        IEnumerable<Rule> GetRules(string serverIP);
    }
}