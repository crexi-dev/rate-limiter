using System.Collections.Generic;

namespace RateLimiter
{
    public class LimitRule
    {
        public List<string> EndPoints { get; set; }
        public RuleType RuleType { get; set; }
        public Period Period { get; set; }
        public long Value { get; set; }
    }
}
