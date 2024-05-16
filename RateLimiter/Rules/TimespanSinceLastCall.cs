using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public class TimespanSinceLastCall : IAllowRequest
    {
        private Dictionary<string, DateTime> lastRequestTimes = new Dictionary<string, DateTime>();
        private TimeSpan appTimeSpanLimit;

        public TimespanSinceLastCall(TimeSpan timeSpanLimit) {
            appTimeSpanLimit = timeSpanLimit;
        }

        public bool IsResourceAllowed(string resource)
        {
            if (!lastRequestTimes.ContainsKey(resource))
            {
                lastRequestTimes[resource] = DateTime.MinValue;
            }

            DateTime lastRequestTime = lastRequestTimes[resource];
            DateTime currentTime = DateTime.Now;

            if ((currentTime - lastRequestTime) > appTimeSpanLimit)
            {
                // If enough time has passed since the last request, allow the request
                lastRequestTimes[resource] = currentTime;
                return true;
            }
            
                // If not enough time has passed, deny the request
                return false;            
        }
    }
}
