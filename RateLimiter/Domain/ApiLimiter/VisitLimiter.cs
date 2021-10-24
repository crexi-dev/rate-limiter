using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.ApiLimiter
{
    public class VisitLimiter
    {
        public string Token { get; set; }

        public (int Resource, string Region) resourceRegion;

    }
}
