using System.Collections.Generic;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Library
{
    public interface IRuleRepository {
        int AddRule(Rule rule);
        Rule GetRule(string resource, string serverIP);
    }
}