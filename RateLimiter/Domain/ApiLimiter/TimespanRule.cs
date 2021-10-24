using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.ApiLimiter
{
    public class TimespanRule
    {
        //public Region Region { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
