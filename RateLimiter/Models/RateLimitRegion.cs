using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RateLimiter.Models
{
    public class RateLimitRegion
    {
        [Key]
        public int Id { get; set; }
        public string RegionBase { get; set; }
        public string RegionName { get; set; }
    }
}
