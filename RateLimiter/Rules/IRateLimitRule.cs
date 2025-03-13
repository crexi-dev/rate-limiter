using RateLimiter.Models;

namespace RateLimiter.Rules;

public interface IRateLimitRule
{
    RateLimitResult IsRequestAllowed(string clientId);
}
