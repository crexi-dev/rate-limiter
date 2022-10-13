using RateLimiter.Contract;
using System.Threading.Tasks;

namespace RateLimiter
{
    internal class CompositeLimitStrategy : IRequestLimitStrategy
    {
        private readonly IRequestLimitStrategy[] requestLimitStrategies;

        public CompositeLimitStrategy(params IRequestLimitStrategy[] requestLimitStrategies)
        {
            this.requestLimitStrategies = requestLimitStrategies;
        }

        public async Task<bool> CanPassThroughAsync(Request request)
        {
            foreach (var requestLimitStrategy in requestLimitStrategies)
            {
                if (!await requestLimitStrategy.CanPassThroughAsync(request))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
