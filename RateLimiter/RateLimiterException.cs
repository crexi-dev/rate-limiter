using System;

namespace RateLimiter
{
    public class RateLimiterException : Exception
    {
        public RateLimiterException(string message) : base(message)
        {
        }
    }
}