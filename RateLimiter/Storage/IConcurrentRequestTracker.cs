using System;
using System.Threading.Tasks;
using RateLimiter.Models;

namespace RateLimiter.Storage
{
    public interface IConcurrentRequestTracker
    {
        Task<int> RecordRequestAndCountAsync(RequestContext context, TimeSpan timeWindow);
        
        Task<DateTime?> GetLastRequestTimeAsync(RequestContext context);
        
        Task RecordRequestTimeAsync(RequestContext context);
    }
}