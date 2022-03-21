using RateLimiter.Core.Models.Identity;
using RateLimiter.Core.Models.RateLimit;

namespace RateLimiter.API.Processors;

public interface IRateLimitProcessor
{
    IEnumerable<RateLimitRule> GetMatchingRules(ClientRequestIdentity identity);

    Task<RateLimitCounter> ProcessRequestAsync(ClientRequestIdentity requestIdentity, RateLimitRule rule,
        CancellationToken cancellationToken = default);
}