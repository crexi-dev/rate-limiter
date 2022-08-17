using RateLimiter.Models.Conditions;

namespace RateLimiter.Models.Configuration.Conditions
{
    public abstract class ConditionConfiguration
    {
        public ConditionType Type { get; set; }
    }
}
