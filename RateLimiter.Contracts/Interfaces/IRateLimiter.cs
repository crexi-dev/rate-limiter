using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Contracts.Interfaces
{
    public interface IRateLimiter
    {
        public bool IsRequestAllowed(string accessToken, string resource);
    }
}
