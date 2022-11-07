using RateLimiter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter
{
    /// <summary>
    /// Request limiter using set of underlied sub-limiters to validate request
    /// </summary>
    public class AggreagatedLimiter : ILimiter
    {
        private readonly List<ILimiter> _limiters;

        public AggreagatedLimiter(IEnumerable<ILimiter> limiters)
        {
            _limiters = limiters.ToList();
        }

        public bool Validate(string requestToken)
        {
            return _limiters.All(x => x.Validate(requestToken));
        }
    }
}
