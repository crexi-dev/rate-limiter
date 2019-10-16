using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class RateLimitRule
    {
        public Guid ClientId { get; set; }
        public RequestsPerTimeSpanRule RequestsPerTimeSpanRule { get; set; }
        public TimeSinceLastCallRule TimeSinceLastCallRule { get; set; }
    }

    
}
