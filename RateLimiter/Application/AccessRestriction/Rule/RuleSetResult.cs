using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Application.AccessRestriction.Rule
{
    /// <inheritdoc />
    public class RuleEngineExecutionResult : IRuleEngineExecutionResult
    {
        /// <inheritdoc />
        public bool IsSuccess => RuleResults.All(x => x.IsSuccess);

        /// <inheritdoc />
        public List<IRuleResult> RuleResults { get; set; }
    }
}