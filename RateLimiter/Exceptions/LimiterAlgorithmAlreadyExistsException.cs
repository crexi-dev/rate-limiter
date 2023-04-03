using System;

namespace RateLimiter.Exceptions;

public sealed class LimiterAlgorithmAlreadyExistsException : Exception
{
    private LimiterAlgorithmAlreadyExistsException() { }

    public LimiterAlgorithmAlreadyExistsException(string algorithmName) : base($"Rate limiter already contains {algorithmName} algorithm.")
    { }
}
