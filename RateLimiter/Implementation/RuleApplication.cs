using System.Collections.Generic;

namespace RateLimiter.Implementation;

public class RuleApplication
{
    public string RuleName { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}