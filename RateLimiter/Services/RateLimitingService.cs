using Microsoft.Extensions.Logging;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public class RateLimitingService : IRateLimitingService
    {
        private readonly IRuleProvider _ruleProvider;
        private readonly ILogger<RateLimitingService> _logger;

        public RateLimitingService(IRuleProvider ruleProvider, ILogger<RateLimitingService> logger)
        {
            _ruleProvider = ruleProvider ?? throw new ArgumentNullException(nameof(ruleProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<RateLimiterResult> ValidateRequestAsync(string resource, string token)
        {
            if (string.IsNullOrWhiteSpace(resource))
                throw new ArgumentException("Resource cannot be null or empty.", nameof(resource));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));

            _logger.LogInformation("Checking request for resource: {resource}, token: {token}", resource, token);

            string region = token.Split('-')[0];
            var rules = _ruleProvider.GetRulesForResource(resource, region);

            var checkTasks = rules.Select(rule => Task.Run(() => rule.CheckRule(token))).ToList();

            await Task.WhenAll(checkTasks);

            var errors = checkTasks
                .Where(t => !t.Result.IsAllowed)
                .Select(t => new RateLimitError(t.Result.ErrorMessage, t.Result.ErrorCode))
                .ToArray();

            if (errors.Any())
            {
                _logger.LogWarning("Request for resource: {resource}, token: {token} was blocked: {errors}", resource, token, string.Join(", ", errors.Select(e => e.ErrorCode)));
            }
            else
            {
                _logger.LogInformation("Request for resource: {resource}, token: {token} was allowed.", resource, token);
            }

            return new RateLimiterResult(errors.Length == 0, errors);
        }
    }
}