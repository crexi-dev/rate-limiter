using Microsoft.Extensions.Logging;
using RateLimiter.Enumerators;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public class RateLimiterService
    {

        private readonly RateLimiterOptions _options;
        private readonly IRateLimiterHandlerContextFactory _contextFactory;
        private readonly IRateLimiterHandlerProvider _handlers;
        private readonly IRateLimiterEvaluator _evaluator;
        private readonly ILogger _logger;

        public virtual async Task<RateLimitResult> RateLimitRequestAsync(User user, string requestPath, RateLimiterPolicy policy, IDateTimeWrapper requestDate)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            var result = new RateLimitResult();
            result.Policy = policy;

            var authContext = _contextFactory.CreateContext(policy, policy.Requirements, user, requestDate, requestPath);
            var handlers = await _handlers.GetHandlersAsync(authContext).ConfigureAwait(false);
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(authContext).ConfigureAwait(false);
            }

            if (authContext.RequirementsMet) // Does the Policy match to the user?
            {
                result.Type = _evaluator.Evaluate(authContext);
            }
            else
            {
                result.Type = eRateLimiterResultType.NotApplicable;
            }

            return result;
        }

        public RateLimiterService(IRateLimiterHandlerProvider handlers, 
                                    ILogger<RateLimiterService> logger, 
                                    IRateLimiterHandlerContextFactory contextFactory, 
                                    IRateLimiterEvaluator evaluator)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (contextFactory == null)
            {
                throw new ArgumentNullException(nameof(contextFactory));
            }
            if (evaluator == null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }

            _handlers = handlers;
            _logger = logger;
            _evaluator = evaluator;
            _contextFactory = contextFactory;
        }
    }
}
