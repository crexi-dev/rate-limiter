using System;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class UsRateLimiter : RateLimiter
    {
        public UsRateLimiter(int requestsPerSecond, Func<Task> doCall) : base(doCall)
        {
            var validatedRequestPerSeconds = requestsPerSecond <= 0 ? 1 : requestsPerSecond;
            this.period = 1000 / validatedRequestPerSeconds;
        }
    }
}
