using RateLimiter.Models.Conditions;

namespace RateLimiter.Models.Configuration.Conditions
{
    public sealed class HasRoleConditionConfiguration : ConditionConfiguration
    {
        public HasRoleConditionConfiguration()
        {
            Type = ConditionType.HasRoleCondition;
        }

        public string Role { get; set; }
    }
}
