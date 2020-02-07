using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Enums;

namespace RateLimiter.Models
{
    public class TimeSpanLimitRule : BaseRule
    {
        public TimeSpan TimeSpanLimit { get; set; }

        public TimeSpanLimitRule() : base(RuleTypes.TimeSpanLimiting)
        {
        }
    }
}
