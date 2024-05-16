using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;


namespace RateLimiter.Rules
{
    public class RequestPerTimespan : IAllowRequest
    {
        private Dictionary<string, int> requestCount = new Dictionary<string, int>();
        private int appLimit;
        private TimeSpan appSpan;

        public RequestPerTimespan(int limit, TimeSpan span)
        {
            appLimit = limit;
            appSpan = span;
        }

        public bool IsResourceAllowed(string resource)
        {
            var now = DateTime.Now;
            var expiredKeys = requestCount.Keys.Where(key => (now - DateTime.Parse(key)).TotalSeconds > appSpan.TotalSeconds).ToList();
            foreach (var key in expiredKeys)
            {
                requestCount.Remove(key);
            }

            var currentTime = now.ToString();
            if (!requestCount.ContainsKey(currentTime))
            {
                requestCount[currentTime] = 1;
            }
            else
            {
                requestCount[currentTime]++;
            }

            return requestCount.Values.Sum() <= appLimit;
        }
    }
}
