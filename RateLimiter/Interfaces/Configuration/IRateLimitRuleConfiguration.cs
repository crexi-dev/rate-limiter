using System.Collections.Generic;

namespace RateLimiter.Interfaces.Configuration
{
    /// <summary>
    /// Descibes the configuration for an individual rate-limit rule
    /// </summary>
    public interface IRateLimitRuleConfiguration
    {
        /// <summary>
        /// The class name of the rule
        /// </summary>
        string Type { get; init; }

        /// <summary>
        /// A dictionary of parameters that are supplied to the rule
        /// </summary>
        IDictionary<string, object> Parameters { get; init; }
    }
}
