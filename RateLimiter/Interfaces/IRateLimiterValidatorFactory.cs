using RateLimiter.DataStorageSimulator;

namespace RateLimiter.Interfaces
{
    public interface IRateLimiterValidatorFactory
    {
        IRateLimiterValidator CreateRateLimiterValidator(RateLimiterType type);
    }
}
