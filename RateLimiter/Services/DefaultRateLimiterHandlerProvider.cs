using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public class DefaultRateLimiterHandlerProvider : IRateLimiterHandlerProvider
    {
        private readonly Task<IEnumerable<IRateLimiterHandler>> _handlersTask;

        public DefaultRateLimiterHandlerProvider(IEnumerable<IRateLimiterHandler> handlers)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }

            _handlersTask = Task.FromResult(handlers);
        }

        public Task<IEnumerable<IRateLimiterHandler>> GetHandlersAsync(RateLimiterHandlerContext context) => _handlersTask;
    }
}
