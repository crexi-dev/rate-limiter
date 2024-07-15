using RateLimiter.Interfaces.Models;

namespace RateLimiter.Models
{
    public record RateLimitRuleResult(bool Proceed, string? Error) : IRateLimitRuleResult;
}
