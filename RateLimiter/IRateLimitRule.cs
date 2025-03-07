/*
 * Author   Joel Hernandez James
 * Current Date  3/6/2025
 * Class    IRateLimitRule
 */

using System;

namespace RateLimiter
{
    /// <summary>
    /// A blueprint for all rate limiting rules. Any class that wants to be a rate limiting rule
    /// needs to follow this pattern. Think of it like a contract that all rules must sign.
    /// </summary>
    public interface IRateLimitRule
    {
        /// <summary>
        /// Decides whether to let a request through or block it
        /// </summary>
        /// <param name="token">Who's making the request (their ID or access key)</param>
        /// <param name="resourceId">What they're trying to access</param>
        /// <returns>Green light (true) if allowed, red light (false) if blocked</returns>
        bool IsRequestAllowed(string token, string resourceId);
    }
}
