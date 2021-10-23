using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.ApiLimiter
{
    public class Rule
    {
        public Region Region { get; set; }
        //public Resource Resource { get; set; }
        public int Limit { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
