namespace RateLimiter.Application.AccessRestriction.Rule
{
    /// <inheritdoc />
    class RuleResource : IRuleResource
    {
        /// <inheritdoc />
        public string ResourceName { get; set; }

        /// <inheritdoc />
        public IRule Rule { get; set; }
    }
}