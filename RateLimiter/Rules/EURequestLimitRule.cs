using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class EURequestLimitRule : IAllowRequest
    {
        private Dictionary<string, DateTime> lastRequestTimes = new Dictionary<string, DateTime>();
        private TimeSpan timeSpanLimit;

        public EURequestLimitRule(TimeSpan timeSpanLimit)
        {
            this.timeSpanLimit = timeSpanLimit;
        }        

        public bool IsResourceAllowed(string resource)
        {
            if (!lastRequestTimes.ContainsKey(resource))
            {
                lastRequestTimes[resource] = DateTime.MinValue;
            }

            DateTime lastRequestTime = lastRequestTimes[resource];
            DateTime currentTime = DateTime.Now;

            if ((currentTime - lastRequestTime) > timeSpanLimit)
            {
                // If enough time has passed since the last request, allow the request
                lastRequestTimes[resource] = currentTime;
                return true;
            }
            else
            {
                // If not enough time has passed, deny the request
                return false;
            }
        }
    }
}
