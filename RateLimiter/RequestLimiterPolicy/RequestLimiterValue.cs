using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RateLimiter.Tests")]
namespace RateLimiter.RequestLimiterPolicy
{
    internal struct RequestLimiterValue
    {
        public List<DateTime> Calls;
        public RequestLimiterValue()
        {
            Calls = [];
        }
    }
}
