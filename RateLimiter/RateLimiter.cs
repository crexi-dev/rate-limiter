using RateLimiter.Services;

namespace RateLimiter;

public class RateLimiter
{
    private readonly RateLimiterManager _rateLimiterManager;

    public RateLimiter(RateLimiterManager rateLimiterManager)
    {
        _rateLimiterManager = rateLimiterManager;
    }

    public bool AllowRequest(string clientId, string resource)
    {
        return _rateLimiterManager.IsRequestAllowed(clientId, resource).IsAllowed;
    }
}
