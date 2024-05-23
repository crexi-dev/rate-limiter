using System.Threading.Tasks;

namespace RateLimiter.Rules.Interfaces;

public interface IRateLimitRule
{
    Task<bool> IsRequestAllowedAsync(string clientId, string region);
}