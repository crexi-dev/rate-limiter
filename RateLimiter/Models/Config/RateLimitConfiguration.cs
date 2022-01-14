namespace RateLimiter.Models.Config;

public class RateLimitConfiguration
{
    public IpConfiguration? IpConfiguration { get; set; }

    public ClientConfigKeyConfiguration? ClientKeyConfiguration { get; set; }
}