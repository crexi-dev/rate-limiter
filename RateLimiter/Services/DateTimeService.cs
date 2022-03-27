using RateLimiter.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public class DateTimeService : ITimeService
    {
        public DateTime Now => DateTime.UtcNow;
        public long TimeStamp => Stopwatch.GetTimestamp();
    }
}
