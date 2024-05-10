using RateLimiter.Models;
using System.Collections.Generic;

namespace RateLimiter.Executors
{
    public interface IRuleExecutor
    {
        bool ExecuteRules(IEnumerable<RuleExecuteRequestModel> ruleExecuteRequestModels, string token);
        bool ExecuteRule(RuleExecuteRequestModel ruleExecuteRequestModel, string token);
    }
}
