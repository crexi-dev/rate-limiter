using System;

namespace RateLimiter.Exceptions;

public class RateLimiterConfigurationException : Exception
{
    public RateLimiterConfigurationException(string message) : base(message)
    {
    }

    public RateLimiterConfigurationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}