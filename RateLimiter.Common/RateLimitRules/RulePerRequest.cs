using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.RateLimitRules
{
    public class RulePerRequest : IRateLimitRule
    {
        public long TimeSpanInMilliseconds { get; set; }
        public int NumberOfRequests { get; set; }

        public bool IsAllowed(List<RequestEvent> requestEvents)
        {

            if (requestEvents == null || requestEvents.Count() < 2)
                return true;

            var events = requestEvents.TakeLast(NumberOfRequests + 1);
            var current = events.Last().RequestDate;
            var lastEvent = events.First().RequestDate;

            return (current - lastEvent).TotalMilliseconds > TimeSpanInMilliseconds;

        }

    }
}