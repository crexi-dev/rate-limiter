using System;

namespace RateLimiter.Interfaces
{
    public interface IEvaluate
    {
        bool CanGoThrough(DateTimeOffset requestDateTimeOffset);
    }
}
