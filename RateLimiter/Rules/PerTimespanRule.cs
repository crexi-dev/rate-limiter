using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class PerTimespanRule : IRule
    {
        private readonly int maxRequests;
        private readonly TimeSpan timespan;

        public PerTimespanRule(int maxRequests, TimeSpan timespan)
        {
            this.maxRequests = maxRequests;
            this.timespan = timespan;
        }
        
        public bool IsAllowed(Queue<RequestModel> requestModelList)
        {
            int count = 0;
            
            while (requestModelList.Count > 0 && DateTime.UtcNow - requestModelList.Peek().RequestDate < timespan)
            {
                requestModelList.Dequeue();
                count++;
            }
             
            return count <= maxRequests;
        }
    }
}
