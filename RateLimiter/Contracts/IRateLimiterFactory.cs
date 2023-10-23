namespace RateLimiter.Contracts
{
    public interface IRateLimiterFactory
    {
        IRateLimiter GenerateRateLimitter(); 
    }
}
