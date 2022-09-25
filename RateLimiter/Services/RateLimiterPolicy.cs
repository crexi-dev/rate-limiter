using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Services
{
    public class RateLimiterPolicy
    {
        public string Name { get; set; } = "";

        public RateLimits RateLimits {get; set;}

        public IList<IRateLimiterRequirement> Requirements { get; set; }


    }
}
