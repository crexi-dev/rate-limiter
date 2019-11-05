using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Model
{
    public class RateLimiterParameters
    {
        public int LimitRequests { get; set; }

        public int LimitsRequestsPerResource { get; set; }

        public TimeSpan? TimeLimit{ get; set; }

        public TimeSpan? TimeLimitPerResource { get; set; }
        
        public TimeSpan? DifferenceBetweenRequests { get; set; }
    }
}
