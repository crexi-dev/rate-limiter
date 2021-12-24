using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public interface ILimitRule
    {
        bool Check(SimpleHttpContext context, IEnumerable<SimpleHttpContext>? requests);
    }
}
