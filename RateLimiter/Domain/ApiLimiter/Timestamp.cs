using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.ApiLimiter
{
    public class Timestamp : ITimestamp
    {
        public long GetTimestamp()
        {
            return Stopwatch.GetTimestamp();
        }
    }
}
