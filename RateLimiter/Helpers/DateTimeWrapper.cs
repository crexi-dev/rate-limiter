using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Helpers
{
    public class DateTimeWrapper : IDateTimeWrapper
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
