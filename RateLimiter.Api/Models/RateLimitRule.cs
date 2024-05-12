namespace RateLimiter.Api.Models;

public class RateLimitRule
{
    public string Name { get; set; }
    public int MaxRequests { get; set; }
    public int TimeSpanSeconds { get; set; }
}