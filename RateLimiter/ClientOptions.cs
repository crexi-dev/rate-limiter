using RateLimiter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class ClientOptions
    {
        public string SectionName { get; set; } = "ClientOptions";

        public string ClientId { get; set; }

        public int Limit { get; set; }

        public int Period { get; set; }

        public ERoleType RoleType { get; set; }
    }
}
