using RateLimiter.Models.Configuration.Conditions;

namespace RateLimiter.Builders.Configuration
{
    public sealed class IsClientAuthenticatedConditionConfigurationBuilder
    {
        public IsClientAuthenticatedConditionConfiguration Build() => new()
        {
            IsClientAuthenticated = true
        };
    }
}
