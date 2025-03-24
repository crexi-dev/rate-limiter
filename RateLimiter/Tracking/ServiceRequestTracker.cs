using RateLimiter.Data;
using RateLimiter.Model.Resource;
using RateLimiter.Model.Response;
using System;

namespace RateLimiter.Tracking
{
    public class ServiceRequestTracker : IServiceRequestTracker
    {
        private readonly IRepositoryRateLimiter Repo;

        public ServiceRequestTracker(IRepositoryRateLimiter repo) 
        {
            Repo = repo;
        }

        public void Track(ResourceRequest request, ResponseRateLimitValidation response)
        {
            Repo.LogResourceRequestAttempt(request, response);
        }
    }
}
