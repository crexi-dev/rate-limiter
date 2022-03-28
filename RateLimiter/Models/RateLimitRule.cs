using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RateLimiter.Models
{
    public class RateLimitRule
    {
        [Key]
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public int RegionId { get; set; }
        public TimeSpan TimeSpanAllowed { get; set; }
        public int NumberRequestsAllowed { get; set; }
        public RateLimitRuleType RuleType { get; set; }
    }
}
