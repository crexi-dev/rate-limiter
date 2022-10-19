using RateLimiter.Model;

namespace RateLimiter.RulesEngine.Interfaces
{
    public interface IRateLimiterRule
    {
        bool IsEnabled(ClientRequest request);
        bool Validate(ClientRequest request);
    }
}
