using System.Collections.Generic;

namespace RateLimiter.RulesEngine.Repository
{
    public interface IRuleRepository {
        IEnumerable<Rule> GetRules(string serverIP);
    }
}