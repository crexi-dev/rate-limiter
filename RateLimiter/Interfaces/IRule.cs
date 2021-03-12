using System;
using System.Collections.Generic;

namespace RateLimiter.Interfaces
{
    public interface IRule
    {
        bool Verify(Stack<DateTimeOffset> requestTimes, DateTimeOffset current);
    }
}
