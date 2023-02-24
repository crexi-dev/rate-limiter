using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public interface IRateLimiter
    {
        Task<bool> Check(string resource, string token);
    }
}
