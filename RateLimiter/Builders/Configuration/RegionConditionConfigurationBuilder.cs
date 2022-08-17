using RateLimiter.Models.Configuration.Conditions;

namespace RateLimiter.Builders.Configuration
{
    public sealed class RegionConditionConfigurationBuilder
    {
        public RegionConditionConfiguration Build() => new()
        {
            Region = "USA"
        };
    }
}
