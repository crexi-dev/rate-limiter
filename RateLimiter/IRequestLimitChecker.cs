using RateLimiter.DataModel;

namespace RateLimiter
{
    public interface IRequestLimitChecker
    {
        bool CanProcessRequest(RequestData requestData);
    }
}
