using RateLimiter.Classes;
using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class GeoLocationLimit : IRule
    {
        public bool Validate(IClientRequest request, IRateLimiter rate_limiter)
        {
            return request.GeoLocation == GeoLocation.US ?
                new RequestsPerTimespanLimit().Validate(request, rate_limiter) :
                new TimespanSinceLastCallLimit().Validate(request, rate_limiter);
        }
    }
}
