namespace RateLimiter.Api.Models;

public class RateLimitingOptions
{
    public List<RateLimitRule> GeneralRules { get; set; }
    public List<CountrySpecificRule> CountrySpecificRules { get; set; }
}