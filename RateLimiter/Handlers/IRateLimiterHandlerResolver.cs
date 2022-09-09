namespace RateLimiter.Handlers
{
    public interface IRateLimiterHandlerResolver
    {
        IRateLimiterHandler Resolve(string clientKey);
    }
}
