using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Enums;

namespace RateLimiter.Models
{
    public class FrequencyLimitRule : BaseRule
    {
        public int NumberOfRequests { get; set; }
        public TimeSpan TimeSpanLimit { get; set; }

        public FrequencyLimitRule() : base(RuleTypes.FrequencyLimiting)
        {
        }
    }
}
