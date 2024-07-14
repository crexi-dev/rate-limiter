using RateLimiter.Models;

namespace RateLimiter;

/// <summary>
/// A service, providing information about count of requests that clients made to the endpoint
/// </summary>
public interface IRateLimitingService
{
    RateLimitCounter GetRateLimitCounter(ClientRequestIdentity identity, RateLimitRule rule);
}
