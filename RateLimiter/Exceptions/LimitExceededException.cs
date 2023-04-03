using System;

namespace RateLimiter.Exceptions;

public sealed class LimitExceededException : Exception
{
    private LimitExceededException() { }

    public LimitExceededException(string api, string token) : base($"Request limit exceeded for api '{api}' and user with token {token}")
    { }
}
