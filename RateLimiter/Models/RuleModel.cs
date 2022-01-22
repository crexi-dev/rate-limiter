using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public class RuleModel
    {
        public int RequestFrequency { get; set; }
        public TimeSpan FromLastCallTimePassed { get; set; }
    }
}
