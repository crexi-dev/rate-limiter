using System;
using RateLimiter.Models;

namespace RateLimiter.Helpers
{
    public class RuleBuilder
    {
        private Rule _rule = new ();

        public RuleBuilder AddMaxCount(int maxCount)
        {
            _rule.RequestsMaxCount = maxCount;
            return this;
        }
        public RuleBuilder AddTimeSpan(TimeSpan timeSpan)
        {
            _rule.TimeSpan = timeSpan;
            return this;
        }
        
        public Rule Build()
        {
            return _rule;
        }
    }
}
