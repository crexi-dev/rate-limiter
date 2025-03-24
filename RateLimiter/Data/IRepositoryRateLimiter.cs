using RateLimiter.Model.Data;
using RateLimiter.Model.Resource;
using RateLimiter.Model.Resource.Config;
using RateLimiter.Model.Response;
using System;
using System.Collections.Generic;

namespace RateLimiter.Data
{
    public interface IRepositoryRateLimiter
    {
        ResourceConfig GetResourceConfig(EnumResource resource);
        int GetResourceRequestCount(EnumResource resource, string token, TimeSpan lookbackTimespan);
        DateTime GetLastResourceRequest(EnumResource resource, string token);
        void CreateResourceConfig(ResourceConfig resourceConfig);
        void LogResourceRequestAttempt(ResourceRequest request, ResponseRateLimitValidation response);
        List<ResourceRequestLog> GetRequestLogAll();
    }
}
