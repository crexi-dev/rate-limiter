using System;

namespace RateLimiter.Exceptions;

public sealed class NoLimiterAlgorithmException : Exception
{
    public NoLimiterAlgorithmException() :base("Rate limiter must contains at least one algoryithm.")
    { }
}
