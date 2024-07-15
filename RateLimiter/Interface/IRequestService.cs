using RateLimiter.Interface;

namespace RateLimiter.Interface
{
    public interface IRequestService
    {
        IClient GetClient(string token);
    }
}