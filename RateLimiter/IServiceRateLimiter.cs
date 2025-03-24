using RateLimiter.Model.Data;
using RateLimiter.Model.Resource;
using RateLimiter.Model.Response;
using System.Collections.Generic;

namespace RateLimiter
{
    public interface IServiceRateLimiter
    {
        ResponseRateLimitValidation Validate(ResourceRequest request);
        List<ResourceRequestLog> GetRequestLogAll();
    }
}
