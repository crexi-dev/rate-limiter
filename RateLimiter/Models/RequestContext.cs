using System;

namespace RateLimiter.Models
{
    public class RequestContext
    {
        public string ClientToken { get; set; } = string.Empty;
        
        public string ResourcePath { get; set; } = string.Empty;
        
        public string Region { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}