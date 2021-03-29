using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.Exceptions
{
    public class LimitExceededException : Exception
    {
        public LimitExceededException(string token)
            : base($"({token}) exceeded request limit.")
        {
        }
    }
}
