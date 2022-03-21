using RateLimiter.Core.Models.Identity;
using RateLimiter.Core.Models.RateLimit;

namespace RateLimiter.API.CounterKeyBuilder;

public interface IClientCounterKeyBuilder
{
    string Build(ClientRequestIdentity requestIdentity, RateLimitRule rule);
}