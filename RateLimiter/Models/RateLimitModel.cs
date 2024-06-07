namespace RateLimiter.Models;

public class RateLimitModel
{
    /// <summary>
    /// Endpoint
    /// </summary>
    public string Endpoint { get; set; }
    /// <summary>
    /// Region
    /// </summary>
    public Region[] Regions { get; set; }
    /// <summary>
    /// Rate limit period as in 1s, 1m, 1h, 1d
    /// </summary>
    public string Period { get; set; }
}