using Microsoft.Extensions.Options;
using RateLimiter.Core.Models.Identity;
using RateLimiter.Core.Models.RateLimit;

namespace RateLimiter.API.CounterKeyBuilder;

public class ClientCounterKeyBuilder : IClientCounterKeyBuilder
{
    private readonly RateLimitOptions _options;

    public ClientCounterKeyBuilder(IOptions<RateLimitOptions> options)
    {
        _options = options.Value;
    }

    public string Build(ClientRequestIdentity requestIdentity, RateLimitRule rule)
    {
        return $"{_options.RateLimitCounterPrefix}_{requestIdentity.ClientId}_{rule.Period}";
    }
}