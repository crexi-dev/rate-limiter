using RateLimiter.Interface;
using System;


namespace RateLimiter.Model
{
    public class ResourceRequest : IResourceRequest
    {
        public string Resource { get; set; }
        public DateTime DateTime { get; set; }
    }
}
