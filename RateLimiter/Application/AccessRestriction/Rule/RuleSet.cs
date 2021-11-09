using System.Collections.Generic;

namespace RateLimiter.Application.AccessRestriction.Rule
{
    /// <inheritdoc />
    public class RuleSet : IRuleSet
    {
        ///<inheritdoc />
        public string Description { get; set; }

        ///<inheritdoc />
        public string Name { get; set; }

        ///<inheritdoc />
        public List<IRule> Rules { get; set; } = new();
    }
}