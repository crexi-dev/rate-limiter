using System.Collections.Generic;

namespace RateLimiter.RulesEngine
{
    public interface IRulesEngine {
        int Create(Rule rule);
        IEnumerable<Rule> GetRules(string serverIP);
        // bool Match(string test, Func<Rule, bool> verifier);
    }
}