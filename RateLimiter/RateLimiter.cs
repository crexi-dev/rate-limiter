/*
 * Author   Joel Hernandez James
 * Current Date  3/6/2025
 * Class    RateLimiter
 */

using System;
using System.Collections.Generic;

namespace RateLimiter
{
    /// <summary>
    /// The main traffic cop that controls access to resources based on rate limits.
    /// Think of it as the gatekeeper that decides who gets in and who has to wait.
    /// </summary>
    public class RateLimiter
    {
        private readonly RequestTracker _requestTracker;
        private readonly Dictionary<string, IRateLimitRule> _resourceRules;

        /// <summary>
        /// Creates a fresh rate limiter with no rules yet
        /// </summary>
        public RateLimiter()
        {
            _requestTracker = new RequestTracker();
            _resourceRules = new Dictionary<string, IRateLimitRule>();
        }

        /// <summary>
        /// Assigns a specific rule to control access to a particular resource
        /// </summary>
        /// <param name="resourceId">Which resource to protect</param>
        /// <param name="rule">The rule that decides who can access it and when</param>
        public void SetRuleForResource(string resourceId, IRateLimitRule rule)
        {
            if (string.IsNullOrEmpty(resourceId))
                throw new ArgumentException("Resource ID cannot be null or empty", nameof(resourceId));
            
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            _resourceRules[resourceId] = rule;
        }

        /// <summary>
        /// The main decision maker - checks if a user can access a resource right now
        /// </summary>
        /// <param name="token">Who's trying to get access (their ID)</param>
        /// <param name="resourceId">What they're trying to access</param>
        /// <returns>Green light (true) if they can proceed, red light (false) if they need to wait</returns>
        public bool IsRequestAllowed(string token, string resourceId)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token cannot be null or empty", nameof(token));
            
            if (string.IsNullOrEmpty(resourceId))
                throw new ArgumentException("Resource ID cannot be null or empty", nameof(resourceId));

            // Check if this resource has any special access rules
            if (_resourceRules.TryGetValue(resourceId, out var rule))
            {
                // Ask the rule if this request should be allowed
                bool isAllowed = rule.IsRequestAllowed(token, resourceId);
                
                return isAllowed;
            }
            
            // No special rules for this resource? Open access!
            return true;
        }

        /// <summary>
        /// Provides access to the request history tracker
        /// </summary>
        /// <returns>The tracker that remembers all request history</returns>
        public RequestTracker GetRequestTracker()
        {
            return _requestTracker;
        }
    }
}
