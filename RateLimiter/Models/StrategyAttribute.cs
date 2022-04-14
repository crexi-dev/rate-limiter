using System;

namespace RateLimiter.Models
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class LimitStrategyAttribute : Attribute
    {
        public LimitStrategy Strategy { get; private set; }

        public LimitStrategyAttribute(LimitStrategy strategy)
        {
            Strategy = strategy;
        }
    }
}
