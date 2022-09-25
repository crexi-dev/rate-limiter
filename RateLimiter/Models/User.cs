using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = "";

        public string Locale { get; set; } = "";

        public List<string> Claims { get; set; } = new List<string>();
    }
}
