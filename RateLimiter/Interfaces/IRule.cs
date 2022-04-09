using RateLimiter.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.Interfaces
{
    public interface IRule
    {
        public bool IsRuleValid(ICacheService cache, string token);
    }
}
