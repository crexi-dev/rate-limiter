using RateLimiterMy.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    internal class StabRule : IRule
    {
        public bool Validate(IRequest request)
        {
            return true;
        }
    }
}
