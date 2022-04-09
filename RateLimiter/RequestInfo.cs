using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public class RequestInfo
    {
        public DateTime FirstRequest { get; set; }

        public DateTime LastRequest { get; set; }

        public int Count { get; set; }
    }
}
