using RateLimiter.Models;

namespace RateLimiter.Interfaces
{
    public interface IRule
    {
        RuleCheckResult CheckRule(string token);
    }
}
