using RateLimiter.Domain.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Application.Interfaces
{
    public interface IRequestLimitService
    {
        void Evaluate(RequestContext context);
    }
}
