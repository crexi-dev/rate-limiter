using Microsoft.Extensions.Logging;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using RateLimiter.Rules;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Engine
{
    /// <summary>
    /// A rules engine that checks the request sequentially against a list of rules, passing the result from one rule to the next
    /// ("chaining"). For improved speed, we could implement a different rules engine that processes all the rules in parallel, 
    /// but that would be less flexible as it would not support chaining.
    /// </summary>
    internal class SequentialRateLimiterRulesEngine : IRateLimiterRulesEngine
    {
        private readonly RateLimiterBaseRule[] _rules;
        private readonly ILogger _logger;

        public SequentialRateLimiterRulesEngine(IRateLimiterRulesEngineOptions options, ILogger<SequentialRateLimiterRulesEngine> logger)
        {
            _rules = options.Rules;
            _logger = logger;
        }

        public async Task<bool> CanProcessAsync(RequestInfo request, CancellationToken cancellationToken)
        {
            try
            {
                RequestState state = RequestState.Unhandled;
                foreach (var rule in _rules)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // We do not do short circuit evaluation since every applicable rule must be run for each request.
                    // This is necessary as each rule may keep track of the number of requests that have been received.
                    state = await rule.GetRequestStateAsync(request, state, cancellationToken);
                }
                // If no rules are denied, you can process the request
                return state != RequestState.Denied;
            }
            catch (Exception ex)
            {
                // Log Exceptions
                _logger.LogError(ex, "Error processing the request");
                // If there's an exception applying the rules, assume the request cannot be processed
                return false;
            }
        }
    }
}
