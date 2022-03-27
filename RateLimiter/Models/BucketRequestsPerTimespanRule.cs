using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class BucketRequestsPerTimespanRule
    {
        public long? WindowStartTime { get; set; }
        public int PrevRequestsCount { get; set; }
        public int RequestsCount { get; set; }
    }
}
