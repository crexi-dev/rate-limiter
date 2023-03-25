using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Handlers
{
    public interface ILimiterHandler
    {
        bool TryProcessRequest(string clientId);
    }
}
