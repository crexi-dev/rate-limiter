using RateLimiter.Configuration;

namespace RateLimiter.Interfaces.Models
{
    /// <summary>
    /// Stores the rules to be associated with an endpoint/verbs combination
    /// </summary>
    /// <remarks>
    /// Could have different endpoints with the same PathPattern but different HTTP verbs
    /// </remarks>
    public interface IEndpoint
    {
        /// <summary>
        /// A regular expression that indicates a path to which the rules should apply
        /// </summary>
        string PathPattern { get; init; }

        /// <summary>
        /// Indicates which HTTP verbs to match
        /// Examples: delete = 1, get = 2, patch = 4, post = 8, put = 16, all = 31
        /// </summary>
        HttpVerbFlags Verbs { get; init; }

        /// <summary>
        /// A collection of rules to apply to the path/verbs supplied
        /// </summary>
        RateLimitRuleConfiguration[] Rules { get; init; }
    }
}
