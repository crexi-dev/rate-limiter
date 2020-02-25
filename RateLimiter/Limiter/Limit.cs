using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Limiter
{
    public class Limit
    {
        // Delay = 5, Count = 3 - type A, 3 requests max per 5 seconds
        // Delay = 2, Count = 1 - type B, 1 request max per 2 seconds

        public int Delay { get; private set; }
        public int Count { get; private set; }

        public Limit(int delay, int count)
        {
            Delay = delay;
            Count = count;
        }
    }
}
