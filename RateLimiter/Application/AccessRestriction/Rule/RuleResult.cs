namespace RateLimiter.Application.AccessRestriction.Rule
{
    /// <inheritdoc />
    public class RuleResult : IRuleResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rule"></param>
        public RuleResult(IRule rule)
        {
            Rule = rule;
        }

        /// <inheritdoc />
        public string FailMessage { get; set; }

        /// <inheritdoc />
        public bool IsSuccess { get; set; }

        /// <inheritdoc />
        public IRule Rule { get; set; }
    }
}