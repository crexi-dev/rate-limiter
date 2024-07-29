namespace RateLimiter;

public class UserSettings
{
    public string Id { get; set; }
    public Region Region { get; set; }
    public ServiceTier ServiceTier { get; set; }
}

public enum Region
{
    None,
    US,
    EU
}

public enum ServiceTier
{
    None,
    Basic,
    Pro,
    Enterprise
}