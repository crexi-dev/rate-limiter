/*
 * Author   Joel Hernandez James
 * Current Date  3/6/2025
 * Class    RequestTracker
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RateLimiter
{
    /// <summary>
    /// Keeps a record of who made requests and when they happened.
    /// Like a logbook that remembers all the requests that come through.
    /// </summary>
    public class RequestTracker
    {
        // Our memory storage - like a filing cabinet where we store timestamps
        // We organize by who made the request and what they accessed
        // Format: "userID_resourceID" → list of when their requests happened
        private readonly ConcurrentDictionary<string, List<DateTime>> _requestHistory;

        /// <summary>
        /// Creates a fresh, empty request tracker ready to start logging requests
        /// </summary>
        public RequestTracker()
        {
            _requestHistory = new ConcurrentDictionary<string, List<DateTime>>();
        }

        // Writes down a new request in our logbook
        // Like adding an entry: "User X accessed Resource Y at this time"
        public void RecordRequest(string token, string resourceId)
        {
            // Create a unique label by combining who and what
            string key = GetKey(token, resourceId);
            
            // Note down exactly when this happened
            DateTime now = DateTime.UtcNow;
            
            // Add this timestamp to our records
            _requestHistory.AddOrUpdate(
                key,
                // If this is their first request, start a new page in the logbook
                _ => new List<DateTime> { now },
                // If they've been here before, just add another entry to their page
                (_, timestamps) =>
                {
                    timestamps.Add(now);
                    return timestamps;
                });
        }

        // Looks up all the times a specific user accessed a specific resource
        // Like flipping through the logbook to find all entries for a particular user
        public List<DateTime> GetRequestHistory(string token, string resourceId)
        {
            string key = GetKey(token, resourceId);
            
            // Find their history or return an empty list if they've never been here
            return _requestHistory.GetValueOrDefault(key, new List<DateTime>());
        }

        /// <summary>
        /// Cleans out old entries from our records that we don't need anymore
        /// </summary>
        /// <param name="token">Who made the requests (their ID)</param>
        /// <param name="resourceId">What they accessed</param>
        /// <param name="timespan">How far back to keep records (older ones get tossed)</param>
        public void CleanupHistory(string token, string resourceId, TimeSpan timespan)
        {
            string key = GetKey(token, resourceId);
            
            // Figure out the cutoff point - anything older gets removed
            DateTime cutoff = DateTime.UtcNow.Subtract(timespan);
            
            // Throw away all the timestamps that are too old
            if (_requestHistory.TryGetValue(key, out var timestamps))
            {
                timestamps.RemoveAll(timestamp => timestamp < cutoff);
            }
        }

        /// <summary>
        /// Creates a lookup key by combining user ID and resource ID
        /// </summary>
        /// <param name="token">Who's making the request (their ID)</param>
        /// <param name="resourceId">What they're trying to access</param>
        /// <returns>A combined key like "userID_resourceID"</returns>
        private string GetKey(string token, string resourceId)
        {
            return $"{token}_{resourceId}";
        }
    }
}
