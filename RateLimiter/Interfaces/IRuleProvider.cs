using RateLimiter.Services;
using System.Collections.Generic;

namespace RateLimiter.Interfaces
{
    public interface IRuleProvider
    {
        /// <summary>
        /// Adds a rate limiting rule to the provider for a specific resource and region.
        /// </summary>
        /// <param name="resource">The name of the resource the rule applies to.</param>
        /// <param name="region">The region the rule applies to.</param>
        /// <param name="rule">The rate limiting rule to add.</param>
        /// <returns>The current <see cref="IRuleProvider"/> instance, allowing for method chaining.</returns>
        IRuleProvider AddRule(string resource, string region, IRule rule);

        /// <summary>
        /// Retrieves a list of rate limiting rules for a specific resource and region.
        /// </summary>
        /// <param name="resource">The name of the resource.</param>
        /// <param name="region">The region.</param>
        /// <returns>A list of rate limiting rules for the specified resource and region. Returns an empty list if no rules are found.</returns>
        List<IRule> GetRulesForResource(string resource, string region);
    }
}