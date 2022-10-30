using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Interfaces
{
    public interface IRateLimitRule
    {
        void ValidateRequest();
    }
}
