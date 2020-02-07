using RateLimiter.Enums;
using System;
using System.Collections.Generic;

namespace RateLimiter.Utilities
{
    public static class HistoryHelper
    {
        private static readonly Dictionary<Guid, Queue<DateTime>> TimeSpanHistory = new Dictionary<Guid, Queue<DateTime>>();
        private static readonly Dictionary<Guid, Queue<DateTime>> FrequencyHistory = new Dictionary<Guid, Queue<DateTime>>();

        private static void Save(IDictionary<Guid, Queue<DateTime>> history, Guid token, DateTime currentDate)
        {
            if (history.ContainsKey(token))
            {
                history[token].Enqueue(currentDate);
            }
            else
            {
                var queue = new Queue<DateTime>();
                queue.Enqueue(currentDate);

                history.Add(token, queue);
            }
        }

        private static void Cleanup(Dictionary<Guid, Queue<DateTime>> history, Guid token, DateTime currentDate, TimeSpan timeSpanLimit)
        {
            if (history.ContainsKey(token))
            {
                while (currentDate - history[token].Peek() > timeSpanLimit)
                {
                    history[token].Dequeue();
                }
            }
        }

        /// <summary>
        /// Checks if the max number of requests per a given <paramref name="timeSpanLimit"/> is reached for current <paramref name="token"/>.
        /// </summary>
        /// <param name="token">Current token associated with the request.</param>
        /// <param name="numberOfRequests">Number of requests allowed per given period of time.</param>
        /// <param name="timeSpanLimit">Period of time the requests are allowed to be made.</param>
        /// <returns>Boolean value indicating if the request has been made within the limitations.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool ValidateFrequency(Guid token, int numberOfRequests, TimeSpan timeSpanLimit)
        {
            var currentDate = DateTime.UtcNow;

            if (FrequencyHistory.ContainsKey(token))
            {
                // Calling Cleanup to remove all history items older than timeSpanLimit.
                Cleanup(FrequencyHistory, token, currentDate, timeSpanLimit);

                // The number of remaining items in the history after the Cleanup should be less than numberOfRequests.
                if (numberOfRequests <= FrequencyHistory[token].Count)
                {
                    return false;
                }
            }

            Save(FrequencyHistory, token, currentDate);

            return true;
        }

        /// <summary>
        /// Checks if <paramref name="timeSpanLimit"/> has passed since the last request with <paramref name="token"/> was made./>
        /// </summary>
        /// <param name="token">Current token associated with the request.</param>
        /// <param name="timeSpanLimit">The interval requests are allowed to be made.</param>
        /// <returns>Boolean value indicating if the request has been made within the limitations.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool ValidateTimeSpan(Guid token, TimeSpan timeSpanLimit)
        {
            var currentDate = DateTime.UtcNow;

            if (!TimeSpanHistory.ContainsKey(token))
            {
                return true;
            }

            // Cleanup should remove an item if the timeStamp is older than timeSpanLimit, thus making the queue empty.
            Cleanup(TimeSpanHistory, token, currentDate, timeSpanLimit);

            // The queue should be empty if the timeSpanLimit is awaited.
            if (TimeSpanHistory[token].Count != 0)
            {
                return false;
            }

            Save(TimeSpanHistory, token, currentDate);

            return true;
        }
    }
}
