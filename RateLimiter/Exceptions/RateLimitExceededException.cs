using System;

namespace RateLimiter.Exceptions;

public class RateLimitExceededException : Exception
{
    public RateLimitExceededException(string message) : base(message)
    {
    }

    public RateLimitExceededException(string message, Exception innerException) : base(message, innerException)
    {
    }
}