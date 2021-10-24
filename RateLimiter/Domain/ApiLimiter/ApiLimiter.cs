using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.ApiLimiter
{
    public class ApiLimiter
    {
        public Dictionary<string, VisitLimiter> visitLimiters;

        public ApiLimiter()
        {
            visitLimiters = new Dictionary<string, VisitLimiter>();
        }
    }
}
