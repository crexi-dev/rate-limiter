using System.Threading.Tasks;

namespace RateLimiter.Rules.Interfaces;

public interface IRateLimiterRule
{
    string Name { get; }
    Task<bool> IsAllowed(string token, RateLimitOptions options);
}