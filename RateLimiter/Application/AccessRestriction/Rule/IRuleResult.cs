namespace RateLimiter.Application.AccessRestriction.Rule
{

    /// <summary>
    /// Results of executing a rule
    /// </summary>
    public interface IRuleResult
    {
        /// <summary>
        /// Indication of why the rule failed, if applicable
        /// </summary>
        string FailMessage { get; set; }

        /// <summary>
        /// Indicates whether this rule passed or failed.
        /// </summary>
        bool IsSuccess { get; set; }

        /// <summary>
        /// The evaluated rule. Returned for display purposes.
        /// </summary>
        IRule Rule { get; set; }
    }
}