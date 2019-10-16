using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class TimeSinceLastCallRule
    {
        public TimeSpan Period { get; set; }
    }
}
