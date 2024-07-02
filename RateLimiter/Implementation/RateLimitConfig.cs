using System.Collections.Generic;

namespace RateLimiter.Implementation;

public class RateLimitConfig
{
    public List<RuleConfig> Rules { get; set; } = new List<RuleConfig>();
    public List<EndpointConfig> Endpoints { get; set; } = new List<EndpointConfig>();
}