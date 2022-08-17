using RateLimiter.Models.Conditions;

namespace RateLimiter.Models.Configuration.Conditions
{
    public sealed class RegionConditionConfiguration : ConditionConfiguration
    {
        public RegionConditionConfiguration()
        {
            Type = ConditionType.RegionCondition;
        }

        public string Region { get; set; }
    }
}
