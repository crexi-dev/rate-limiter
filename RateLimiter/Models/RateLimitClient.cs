using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RateLimiter.Models
{
    public class RateLimitClient
    {
        [Key]
        public int Id { get; set; }
        public string ClientName { get; set; }
    }

    public class RateLimitClientToken
    {
        public int ClientId { get; set; }
        public string Token { get; set; }
    }
}
