using System.Collections.Generic;
using RateLimiter.Repository;

namespace RateLimiter.RulesEngine
{
    public interface IRulesEngineProxy {
        IEnumerable<Rule> GetRules(string serverIP);
    }
}