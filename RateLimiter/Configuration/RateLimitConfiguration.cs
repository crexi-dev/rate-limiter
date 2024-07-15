using RateLimiter.Interfaces.Configuration;
using RateLimiter.Models;

namespace RateLimiter.Configuration
{
    public class RateLimitConfiguration : IRateLimitConfiguration
    {
        public Endpoint[] Endpoints { get; init; }
    }
}
