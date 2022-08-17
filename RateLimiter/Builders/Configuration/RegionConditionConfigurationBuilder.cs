using RateLimiter.Models.Configuration.Conditions;

namespace RateLimiter.Builders.Configuration
{
    public sealed class RegionConditionConfigurationBuilder
    {
        private string region = "USA";

        public RegionConditionConfigurationBuilder ForRegion(string region)
        {
            this.region = region;
            return this;
        }

        public RegionConditionConfiguration Build() => new()
        {
            Region = region
        };
    }
}
