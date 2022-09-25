using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Interfaces
{
    public interface IDateTimeWrapper
    {
        public DateTime Now { get { return DateTime.Now; } set { } }
    }
}
