namespace RateLimiter.Utilities;

public class Rules
{
    public string Country { get; set; }
    public int? TimeFromLastCall { get; set; }
    public XRequestsPerTimespan? XRequestsPerTimespan { get; set; }
}