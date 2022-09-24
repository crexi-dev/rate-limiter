using System;
using RateLimiter.Model;
namespace RateLimiter
{
    /// <summary>
    /// Interface for RateLimiter
    /// </summary>
    public interface IRateLimiter
    {
        public LimitResult AuthorizeRequest(Request request);
    }
}

