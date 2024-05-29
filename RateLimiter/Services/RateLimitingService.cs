using System;
using System.Linq;
using System.Threading.Tasks;
using RateLimiter.Interfaces;
using RateLimiter.Models;

namespace RateLimiter.Services
{
    /// <summary>
    /// Provides functionality for determining if a request is allowed based on configured rate limiting rules.
    /// </summary>
    public class RateLimitingService : IRateLimitingService
    {
        private readonly IRuleProvider _ruleProvider;

        /// <inheritdoc />
        public RateLimitingService(IRuleProvider ruleProvider)
        {
            _ruleProvider = ruleProvider ?? throw new ArgumentNullException(nameof(ruleProvider));
        }

        /// <inheritdoc />
        public async Task<RateLimiterResult> IsRequestAllowedAsync(string resource, string token)
        {
            string region = token.Split('-')[0];
            var rules = _ruleProvider.GetRulesForResource(resource, region);

            var checkTasks = rules.Select(rule => Task.Run(() => rule.CheckRule(token))).ToList();

            await Task.WhenAll(checkTasks);

            var errors = checkTasks
                .Where(t => !t.Result.IsAllowed)
                .Select(t => new RateLimitError(t.Result.ErrorMessage, t.Result.ErrorCode))
                .ToArray();

            return new RateLimiterResult(errors.Length == 0, errors);
        }
    }
}
