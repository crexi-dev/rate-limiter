using System;
using System.Threading.Tasks;
using RateLimiter.Abstractions;
using RateLimiter.Models;
using RateLimiter.Storage;

namespace RateLimiter.Rules
{
    public class TimeSinceLastCallRule : IRateLimitRule
    {
        private readonly IConcurrentRequestTracker _requestTracker;
        
        public TimeSpan MinInterval { get; set; }
        
        public TimeSinceLastCallRule(IConcurrentRequestTracker requestTracker)
        {
            _requestTracker = requestTracker ?? throw new ArgumentNullException(nameof(requestTracker));
            MinInterval = TimeSpan.FromSeconds(1);
        }
        
        public async Task<bool> ValidateAsync(RequestContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            var lastRequestTime = await _requestTracker.GetLastRequestTimeAsync(context);
            
            if (!lastRequestTime.HasValue)
            {
                await _requestTracker.RecordRequestTimeAsync(context);
                return true;
            }
            
            var timeSinceLastRequest = DateTime.UtcNow - lastRequestTime.Value;
            
            var isAllowed = timeSinceLastRequest >= MinInterval;
            
            if (isAllowed)
            {
                await _requestTracker.RecordRequestTimeAsync(context);
            }
            
            return isAllowed;
        }
    }
}