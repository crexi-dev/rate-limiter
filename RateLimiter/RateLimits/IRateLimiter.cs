using RateLimits.Rules;

namespace RateLimits.RateLimits
{
    public interface IRateLimiter
    {
        bool HasAccess(string accessToken, string resourceName, string region, params IRule[] rules);
    }
}
