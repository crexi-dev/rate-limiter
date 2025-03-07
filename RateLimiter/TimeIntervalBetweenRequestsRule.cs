/*
 * Author   Joel Hernandez James
 * Current Date  3/6/2025
 * Class    TimeIntervalBetweenRequestsRule
 */

using System;
using System.Linq;

namespace RateLimiter
{
    /// <summary>
    /// This rule makes sure users wait a minimum amount of time between requests.
    /// Think of it like a "cooldown period" before allowing the next request.
    /// </summary>
    public class TimeIntervalBetweenRequestsRule : IRateLimitRule
    {
        private readonly RequestTracker _requestTracker;
        private readonly TimeSpan _minInterval;

        /// <summary>
        /// Sets up a new rule that enforces waiting time between requests
        /// </summary>
        /// <param name="requestTracker">Keeps track of when requests happened</param>
        /// <param name="minInterval">How long users need to wait between requests</param>
        public TimeIntervalBetweenRequestsRule(RequestTracker requestTracker, TimeSpan minInterval)
        {
            // Validate parameters
            if (minInterval <= TimeSpan.Zero)
                throw new ArgumentException("Minimum interval must be greater than zero", nameof(minInterval));

            _requestTracker = requestTracker ?? throw new ArgumentNullException(nameof(requestTracker));
            _minInterval = minInterval;
        }

        /// <summary>
        /// Decides if a new request is allowed based on when the last one happened
        /// </summary>
        /// <param name="token">Who's making the request (like their ID or key)</param>
        /// <param name="resourceId">What they're trying to access</param>
        /// <returns>Yes (true) if enough time has passed, No (false) if they need to wait longer</returns>
        public bool IsRequestAllowed(string token, string resourceId)
        {
            // Look up when this user made requests before
            var requestHistory = _requestTracker.GetRequestHistory(token, resourceId);
            
            // First time? No problem, go right ahead!
            if (requestHistory.Count == 0)
                return true;
            
            // Find when their most recent request happened
            DateTime lastRequestTime = requestHistory.Max();
            
            // Figure out how much time has passed since then
            TimeSpan elapsed = DateTime.UtcNow - lastRequestTime;
            
            // If they've waited long enough, allow the request. Otherwise, they need to wait longer
            return elapsed >= _minInterval;
        }
    }
}
