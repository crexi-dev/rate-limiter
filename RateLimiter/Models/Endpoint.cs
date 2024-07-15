using RateLimiter.Configuration;
using RateLimiter.Interfaces.Models;

namespace RateLimiter.Models
{
    public class Endpoint : IEndpoint
    {
        public string PathPattern { get; init; }

        public HttpVerbFlags Verbs { get; init; }

        public RateLimitRuleConfiguration[] Rules { get; init; }
    }
}
