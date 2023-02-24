using System;
using System.Collections.Generic;

namespace RateLimiter.Configuration
{
    public interface IRule
    {
        bool Check(ICollection<DateTime> accessAttempts);
    }
}