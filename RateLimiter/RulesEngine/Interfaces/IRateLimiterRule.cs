using RateLimiter.Model;

namespace RateLimiter.RulesEngine.Interfaces
{
    internal interface IRateLimiterRule
    {
        bool IsEnabled(UserRequest request);
        bool Validate(UserRequest request);
    }
}
