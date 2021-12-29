using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public class CallDetails
    {
        public int UserId { get; set; }

        public DateTime ApiCallTime { get; set; }
    }
}
