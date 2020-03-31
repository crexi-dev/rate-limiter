using System.Collections.Generic;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Library
{
    public interface IRuleRepository {
        IEnumerable<Rule> GetRules(string serverIP);
    }
}