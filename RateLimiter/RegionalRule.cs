/*
 * Author   Joel Hernandez James
 * Current Date  3/6/2025
 * Class    RegionalRule
 */

using System;
using System.Collections.Generic;

namespace RateLimiter
{
    /// <summary>
    /// Different geographic areas we can apply specific rules to
    /// </summary>
    public enum Region
    {
        US,
        EU,
        Other
    }

    /// <summary>
    /// A rule that applies different limits based on where the user is located.
    /// Like having different speed limits in different countries.
    /// </summary>
    public class RegionalRule : IRateLimitRule
    {
        private readonly Dictionary<Region, IRateLimitRule> _regionRules;
        private readonly Func<string, Region> _regionResolver;

        /// <summary>
        /// Creates a new rule that can apply different limits based on region
        /// </summary>
        /// <param name="regionResolver">A function that figures out which region a user belongs to</param>
        public RegionalRule(Func<string, Region> regionResolver)
        {
            _regionResolver = regionResolver ?? throw new ArgumentNullException(nameof(regionResolver));
            _regionRules = new Dictionary<Region, IRateLimitRule>();
        }

        /// <summary>
        /// Assigns a specific rule to apply for users in a particular region
        /// </summary>
        /// <param name="region">Which region this rule applies to</param>
        /// <param name="rule">The specific rule to use for that region</param>
        public void SetRuleForRegion(Region region, IRateLimitRule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            _regionRules[region] = rule;
        }

        /// <summary>
        /// Checks if a request is allowed by first determining the user's region,
        /// then applying the appropriate rule for that region
        /// </summary>
        /// <param name="token">Who's making the request (their ID)</param>
        /// <param name="resourceId">What they're trying to access</param>
        /// <returns>Yes (true) if allowed for their region, No (false) if blocked</returns>
        public bool IsRequestAllowed(string token, string resourceId)
        {
            // Figure out which region this user belongs to
            Region region = _regionResolver(token);
            
            // See if we have a special rule for their region
            if (_regionRules.TryGetValue(region, out var rule))
            {
                // Apply that region's specific rule
                return rule.IsRequestAllowed(token, resourceId);
            }
            
            // No special rule for their region? Let them through
            return true;
        }
    }
}
