using System;
namespace RateLimiter.Model
{
    /// <summary>
    /// Used to identify type for rule evaluation
    /// </summary>
    public enum RateLimitRuleType
    {
        DefaultLimit,
        TokenLimit,
        LevelLimit,
        RegionLimit,
        RegionSpecificLimit,
        LevelSpecificLimit
    }

    /// <summary>
    /// Used to evaluate if API can be accessed
    /// </summary>
    public class RateLimitRule
    {
    #region Constructor
        public RateLimitRule(RateLimitRuleType ruleType, TimeSpan duration, int limit)
        {
            RuleType = ruleType;
            Duration = duration;
            Limit = limit;
        }
        public RateLimitRule(RateLimitRuleType ruleType, TimeSpan duration, int limit, object ruleDetail)
        {
            RuleType = ruleType;
            Duration = duration;
            Limit = limit;
            RuleDetail = ruleDetail;
        }
        #endregion
        #region Properties
        public RateLimitRuleType RuleType { get; set; }
        public TimeSpan Duration { get; set; }
        public int Limit { get; set; }
        public object RuleDetail { get; set; }
    #endregion 
    }
}

