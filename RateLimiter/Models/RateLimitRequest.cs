using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RateLimiter.Models
{
    public class RateLimitRequest
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }
        public int ResourceId { get; set; }
        public DateTime RequestDate { get; set; }
        public int RequestedTimes { get; set; }

    }
}
