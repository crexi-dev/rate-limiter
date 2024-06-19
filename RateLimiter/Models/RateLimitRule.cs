namespace RateLimiter.Models;

public class RateLimitRule
{
    public string? RegionKey { get; set; }
    public RateLimitRuleType Type { get; set; }
    public int? PermitLimit  { get; set; }
    public int? WindowSizeInSeconds { get; set; }
}