using System.Collections.Generic;
using RateLimiter.Rules;

namespace RateLimiter.Models;

public class ResourceRateLimitConfig
{
    public string? Resource { get; set; }
    public List<IRateLimitRule> Rules { get; set; } = new();
}
