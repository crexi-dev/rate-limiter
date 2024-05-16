
namespace RateLimiter.Interfaces
{
    public interface IAllowRequest
    {
        bool IsResourceAllowed(string resource);
    }
}
