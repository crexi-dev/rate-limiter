using RateLimiter.Models.Configuration.Conditions;

namespace RateLimiter.Builders.Configuration
{
    public sealed class HasRoleConditionConfigurationBuilder
    {
        public HasRoleConditionConfiguration Build() => new()
        {
            Role = "UserRole"
        };
    }
}
