using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public class RateLimitToken
    {
        public string Token { get; set; }
        public int ResourceId { get; set; }        
        public int RegionId { get; set; }        
    }
}
