using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.LimitRules
{
    public class RequestPerTimespanLimitRule : ILimitRule
    {
        private readonly int requestCount;
        private readonly TimeSpan timeSpan;

        public RequestPerTimespanLimitRule(int requestCount, TimeSpan timeSpan)
        {
            this.requestCount = requestCount;
            this.timeSpan = timeSpan;
        }

        public bool Check(SimpleHttpContext context, IEnumerable<SimpleHttpContext>? requests)
        {
            return requests?.Where(x => x.RequestDT >= DateTime.Now.Subtract(timeSpan)).Count() < requestCount;
        }
    }
}
