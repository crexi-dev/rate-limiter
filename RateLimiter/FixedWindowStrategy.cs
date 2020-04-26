using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class FixedWindowStrategy : IRateLimiterStrategy
    {
        public bool ApplyStrategy(int userId, int requestId)
        {
            throw new NotImplementedException();
        }
    }
}
