using RateLimiter.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public class TimeStampService : ITimeStampService
    {
        public long TimeStamp => Stopwatch.GetTimestamp();
    }
}
