using System;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class EuRateLimiter : RateLimiter
    {
        public EuRateLimiter(int period, Func<Task> doCall) : base(doCall)
        {
            this.period = period <= 0 ? 1000 : period;
        }
    }
}
