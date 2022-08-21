using System;

namespace RateLimiter.Exceptions
{
    public class RequestsOutOfLimitException : Exception
    {
        public RequestsOutOfLimitException(string message) : base(message)
        {
        }
    }
}