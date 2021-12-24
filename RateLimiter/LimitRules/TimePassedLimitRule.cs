using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.LimitRules
{
    public class TimePassedLimitRule : ILimitRule
    {
        private readonly TimeSpan timeSpan;

        public TimePassedLimitRule(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
        }

        public bool Check(SimpleHttpContext context, IEnumerable<SimpleHttpContext>? requests)
        {
            return requests?.OrderByDescending(x => x.RequestDT).Select(x => x.RequestDT).FirstOrDefault() <= DateTime.Now.Subtract(timeSpan);
        }
    }
}
