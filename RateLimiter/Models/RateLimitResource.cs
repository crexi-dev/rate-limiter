using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RateLimiter.Models
{
    public class RateLimitResource
    {
        [Key]
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string EndpointName { get; set; }
    }
}
