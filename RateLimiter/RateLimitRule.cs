using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public abstract class RateLimitRule : IRateLimiter
    {
        public abstract bool AllowRequest(string clientId);
    }

}
