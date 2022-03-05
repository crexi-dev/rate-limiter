using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.InMemory
{
    class InMemoryRateLimiterRequestService : IRateLimiterRequestService
    {
        private static readonly List<RateLimiterRequest> _internalRequests = new List<RateLimiterRequest>();

        public Task SaveRequestAsync(RateLimiterRequest request)
        {
            _internalRequests.Add(request);
            return Task.CompletedTask;
        }

        public IEnumerable<RateLimiterRequest> GetRequests(string controllerName, string actionName)
        {
            return InternalGetRequests(controllerName, actionName);
        }

        public IEnumerable<RateLimiterRequest> GetRequests(string controllerName, string actionName, DateTime startDate)
        {
            return InternalGetRequests(controllerName, actionName, startDate: startDate);
        }

        public IEnumerable<RateLimiterRequest> GetRequests(string controllerName, string actionName, DateTime startDate, DateTime endDate)
        {
            return InternalGetRequests(controllerName, actionName, startDate: startDate, endDate: endDate);
        }

        public IEnumerable<RateLimiterRequest> GetAllowedRequests(string controllerName, string actionName)
        {
            return InternalGetRequests(controllerName, actionName, wasAllowed: true);
        }

        public IEnumerable<RateLimiterRequest> GetAllowedRequests(string controllerName, string actionName, DateTime startDate)
        {
            return InternalGetRequests(controllerName, actionName, startDate: startDate, wasAllowed: true);
        }

        public IEnumerable<RateLimiterRequest> GetAllowedRequests(string controllerName, string actionName, DateTime startDate, DateTime endDate)
        {
            return InternalGetRequests(controllerName, actionName, startDate: startDate, endDate: endDate, wasAllowed: true);
        }

        private IEnumerable<RateLimiterRequest> InternalGetRequests(string controllerName, string actionName, DateTime? startDate = null, DateTime? endDate = null, bool? wasAllowed = null)
        {
            var result = _internalRequests.Where(r => r.ControllerName == controllerName && r.ActionName == actionName);

            if (startDate.HasValue)
                result = result.Where(r => r.RequestDate >= startDate.Value);

            if (endDate.HasValue)
                result = result.Where(r => r.RequestDate <= endDate.Value);

            if (wasAllowed.HasValue)
                result = result.Where(r => r.Response?.WasAllowed == wasAllowed.Value);

            return result;
        }
    }
}
