using RateLimiter.Enumerators;
using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public class RateLimitResult
    {
        public eRateLimiterResultType Type { get; set; }
        public RateLimiterPolicy Policy { get; set; }
    }
}
