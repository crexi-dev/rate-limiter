using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter
{
    public interface IRateLimiterRequestService
    {
        Task SaveRequestAsync(RateLimiterRequest request);
        IEnumerable<RateLimiterRequest> GetRequests(string controllerName, string actionName);
        IEnumerable<RateLimiterRequest> GetRequests(string controllerName, string actionName, DateTime startDate);
        IEnumerable<RateLimiterRequest> GetRequests(string controllerName, string actionName, DateTime startDate, DateTime endDate);
        IEnumerable<RateLimiterRequest> GetAllowedRequests(string controllerName, string actionName);
        IEnumerable<RateLimiterRequest> GetAllowedRequests(string controllerName, string actionName, DateTime startDate);
        IEnumerable<RateLimiterRequest> GetAllowedRequests(string controllerName, string actionName, DateTime startDate, DateTime endDate);
    }
}
