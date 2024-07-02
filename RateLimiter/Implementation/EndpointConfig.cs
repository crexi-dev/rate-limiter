using System.Collections.Generic;

namespace RateLimiter.Implementation;

public class EndpointConfig
{
    public string Path { get; set; }
    public List<RuleApplication> AppliedRules { get; set; } = new List<RuleApplication>();
}