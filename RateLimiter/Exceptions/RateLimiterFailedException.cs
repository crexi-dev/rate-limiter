using System;
using System.Diagnostics.CodeAnalysis;

namespace RateLimiter.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class RateLimiterFailedException : Exception
    {
        public RateLimiterFailedException() { }

        public RateLimiterFailedException(string message) : base(message) { }

        public RateLimiterFailedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
