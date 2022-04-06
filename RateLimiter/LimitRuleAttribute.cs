using System;

namespace RateLimiter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LimitRuleAttribute : Attribute
    {
        public string LimitRuleName { get; }

        public LimitRuleAttribute(string limitRuleName)
        {
            LimitRuleName = limitRuleName;
        }
    }
}
