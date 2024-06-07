namespace RateLimiter.Models;

public class RequestsPerTimespanRuleModel : RateLimitModel
{
    /// <summary>
    /// Limit
    /// </summary>
    public double Limit { get; set; }
}