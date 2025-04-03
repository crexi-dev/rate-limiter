using System;
using System.Threading.Tasks;
using RateLimiter.Abstractions;
using RateLimiter.Models;
using RateLimiter.Storage;

namespace RateLimiter.Rules
{
    public class RequestCountRule : IRateLimitRule
    {
        private readonly IConcurrentRequestTracker _requestTracker;
        
        public int MaxRequests { get; set; }
        
        public TimeSpan TimeWindow { get; set; }
        
        public RequestCountRule(IConcurrentRequestTracker requestTracker)
        {
            _requestTracker = requestTracker ?? throw new ArgumentNullException(nameof(requestTracker));
            MaxRequests = 100;
            TimeWindow = TimeSpan.FromMinutes(1);
        }
        
        public async Task<bool> ValidateAsync(RequestContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            var requestCount = await _requestTracker.RecordRequestAndCountAsync(context, TimeWindow);
            
            return requestCount <= MaxRequests;
        }
    }
}