using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces
{
    // giving more extensibility by implementing Rules as classes
    public interface IRule
    {
        bool Validate(IClientRequest request, IRateLimiter rate_limiter);
    }
}
