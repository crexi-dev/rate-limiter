using System.Collections.Generic;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine
{
    public interface IRulesEngine {
        int AddRule(Rule rule);
        Rule GetRule(string resource, string serverIP);
        //RateLimitSettings GetRateLimitSettings();
        // bool Match(string test, Func<Rule, bool> verifier);
    }
}