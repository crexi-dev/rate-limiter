namespace RateLimiter;

// configurable via appsettings.json values
// NOTE: public would actually be internal
public class RateLimiterConfig
{
    public int XRequestsPerTimespan { get; set; }
    public int TimeSinceLastCall { get; set; }
}

public class RateLimiterState
{
    public bool? IsAlloweable { get; }

    public RateLimiterState(bool? isAlloweable)
    {
        IsAlloweable = isAlloweable;
    }
}
