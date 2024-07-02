using System.Collections.Generic;

namespace RateLimiter.Implementation;

public class RuleConfig
{
    public string RuleType { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}