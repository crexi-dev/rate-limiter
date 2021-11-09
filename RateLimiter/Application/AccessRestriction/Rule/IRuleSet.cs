using System.Collections.Generic;

namespace RateLimiter.Application.AccessRestriction.Rule
{
    /// <summary>
    /// Collection of rules
    /// </summary>
    public interface IRuleSet
    {
        /// <summary>
        /// Gets or sets a user friendly explanation of the rule set, i.e  'Limit Resource A by region and time span'.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The rules included in this set.
        /// </summary>
        List<IRule> Rules { get; set; }
    }
}
