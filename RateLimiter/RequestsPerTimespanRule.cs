/*
 * Author   Joel Hernandez James
 * Current Date  3/6/2025
 * Class    RequestsPerTimespanRule
 */

using System;

namespace RateLimiter
{
    /// <summary>
    /// A rule that limits how many requests a user can make within a certain time window.
    /// Like saying "you can only make 100 requests per hour" or "5 requests per minute".
    /// </summary>
    public class RequestsPerTimespanRule : IRateLimitRule
    {
        private readonly RequestTracker _requestTracker;
        private readonly int _maxRequests;
        private readonly TimeSpan _timespan;

        /// <summary>
        /// Creates a new rule that limits requests based on quantity and time
        /// </summary>
        /// <param name="requestTracker">Keeps track of when requests happened</param>
        /// <param name="maxRequests">How many requests are allowed in the time window</param>
        /// <param name="timespan">The time window to count requests in (like 1 minute, 1 hour, etc.)</param>
        public RequestsPerTimespanRule(RequestTracker requestTracker, int maxRequests, TimeSpan timespan)
        {
            // Validate parameters
            if (maxRequests <= 0)
                throw new ArgumentException("Maximum requests must be greater than zero", nameof(maxRequests));
            
            if (timespan <= TimeSpan.Zero)
                throw new ArgumentException("Timespan must be greater than zero", nameof(timespan));

            _requestTracker = requestTracker ?? throw new ArgumentNullException(nameof(requestTracker));
            _maxRequests = maxRequests;
            _timespan = timespan;
        }

        /// <summary>
        /// Checks if a request is allowed based on how many requests the user has already made
        /// </summary>
        /// <param name="token">Who's making the request (their ID)</param>
        /// <param name="resourceId">What they're trying to access</param>
        /// <returns>Yes (true) if they haven't hit their limit yet, No (false) if they've used up their quota</returns>
        public bool IsRequestAllowed(string token, string resourceId)
        {
            // First, clean out any old requests that are outside our time window
            _requestTracker.CleanupHistory(token, resourceId, _timespan);
            
            // Get the list of recent requests within our time window
            var requestHistory = _requestTracker.GetRequestHistory(token, resourceId);
            
            // Check if they still have room in their quota
            // If they've made fewer requests than their limit, let them through
            return requestHistory.Count < _maxRequests;
        }
    }
}
