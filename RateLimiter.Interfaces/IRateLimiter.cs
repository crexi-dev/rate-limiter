using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Interfaces
{
    public interface IRateLimiter
    {
        bool ApplyRule(string token);
    }
}
