using RateLimiter.Models.Conditions;

namespace RateLimiter.Models.Configuration.Conditions
{
    public sealed class IsClientAuthenticatedConditionConfiguration : ConditionConfiguration
    {
        public IsClientAuthenticatedConditionConfiguration()
        {
            Type = ConditionType.RegionCondition;
        }

        public bool IsClientAuthenticated { get; set; }
    }
}
