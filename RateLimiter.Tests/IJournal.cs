using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RateLimiter.Tests
{
    public interface IJournal
    {
        void Add(string token, string description);
        IEnumerable<Execution> Get(Func<Execution, bool> predicate);
    }
}