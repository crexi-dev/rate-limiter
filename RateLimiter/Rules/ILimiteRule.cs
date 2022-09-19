using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Rules
{
    public interface ILimiteRule
    {
        bool CanPassNow(Request request);
    }
}
