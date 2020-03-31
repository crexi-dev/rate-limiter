using System.Collections.Generic;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Library
{
    public interface IRulesEngine {
        int Create(Rule rule);
        IEnumerable<Rule> GetRules(string serverIP);
        // bool Match(string test, Func<Rule, bool> verifier);
    }
}