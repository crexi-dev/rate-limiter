using System.Collections.Generic;

namespace RateLimiter.Application.AccessRestriction.Rule
{
    /// <summary>
    /// Result of executing a collection of rules.
    /// </summary>
    public interface IRuleEngineExecutionResult
    {
        /// <summary>
        /// Whether the set of rules passed. A single failure is enough to cause the ruleset to fail.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Collection of individual rule results.
        /// </summary>
        List<IRuleResult> RuleResults { get; set; }
    }
}