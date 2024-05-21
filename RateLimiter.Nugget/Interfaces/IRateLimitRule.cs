namespace RateLimiter.Nugget.Interfaces
{
    public interface IRateLimitRule<out T>
    {
        bool IsRateLimitExceeded(string accesToken);

        List<string> GetRoutes();
    }

    public interface IRateLimitRoute
    {
        List<string> GetRoutes();
    }
}
