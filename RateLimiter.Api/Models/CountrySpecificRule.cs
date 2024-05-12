namespace RateLimiter.Api;

public class CountrySpecificRule
{
    public string Country { get; set; }
    public int? MaxRequests { get; set; }
    public int? TimeBetweenRequestsSeconds { get; set; }
}
