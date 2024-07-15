using System;

namespace RateLimiter.Interface
{
    public interface IResourceRequest
    {
        DateTime DateTime { get; set; }
        string Resource { get; set; }
    }
}