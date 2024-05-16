using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;


namespace RateLimiter.Rules
{

    public class USRequestLimitRule : IAllowRequest
    {
        private Dictionary<string, int> requestCount = new Dictionary<string, int>();
        private int appLimit;
        private TimeSpan appTimeSpanLimit;

        public USRequestLimitRule(int limit, TimeSpan span)
        {
            appLimit = limit;
            appTimeSpanLimit = span;            
        }

        public bool IsResourceAllowed(string resource)
        {
            if (!requestCount.ContainsKey(resource))
            {
                requestCount[resource] = 1;
            }
            else
            {
                requestCount[resource]++;
            }

            if (requestCount[resource] <= appLimit)
            {
                return true;
            }
            else
            {
                // Reset count after time span
                if ((DateTime.Now - DateTime.MinValue) > appTimeSpanLimit)
                {
                    requestCount[resource] = 1;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
