using RateLimiter.Model.Resource;
using RateLimiter.Model.Response;

namespace RateLimiter.Tracking
{
    public interface IServiceRequestTracker
    {
        void Track(ResourceRequest request, ResponseRateLimitValidation response);
    }
}
