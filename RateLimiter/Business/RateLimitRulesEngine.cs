using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RateLimiter.Interfaces.Business;
using RateLimiter.Interfaces.Business.Rules;
using RateLimiter.Interfaces.Configuration;
using RateLimiter.Interfaces.Factories;
using RateLimiter.Interfaces.Models;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RateLimiter.Business
{
    public class RateLimitRulesEngine : IRateLimitRulesEngine
    {
        private readonly List<IRateLimitRuleFactory> _rateLimitRuleFactories = new List<IRateLimitRuleFactory>();

        private readonly IEnumerable<IEndpoint> _endpoints;

        private readonly ILogger<RateLimitRulesEngine> _logger;

        // Store the rules user per endpoint/verbs combination
        private readonly Dictionary<(string, HttpVerbFlags), List<IRateLimitRule>> _rulesLookup =
            new Dictionary<(string, HttpVerbFlags), List<IRateLimitRule>>();

        private readonly Dictionary<string, HttpVerbFlags> _verbLookup = new Dictionary<string, HttpVerbFlags>
        {
            { "delete", HttpVerbFlags.Delete },
            { "get", HttpVerbFlags.Get },
            { "patch", HttpVerbFlags.Patch },
            { "post", HttpVerbFlags.Post },
            { "put", HttpVerbFlags.Put },
        };

        public RateLimitRulesEngine(IRateLimitConfiguration configuration,
            ILogger<RateLimitRulesEngine> logger,
            IRateLimitRuleFactory rateLimitRuleFactory)
        {
            _endpoints = configuration?.Endpoints ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (rateLimitRuleFactory == null)
            {
                throw new ArgumentNullException(nameof(rateLimitRuleFactory));
            }

            _rateLimitRuleFactories.Add(rateLimitRuleFactory);
        }

        public async Task<IRateLimitRuleResult> Run(HttpContext context, IUser user)
        {
            var verbKey = context.Request.Method.ToLower();

            // This is not a supported verb
            if (!_verbLookup.ContainsKey(verbKey))
            {
                return CreateProceedResponse();
            }

            var path = context.Request.Path;
            var endpoint = _endpoints.FirstOrDefault(ep => ep.Verbs.HasFlag(_verbLookup[verbKey]) && IsMatch(ep.PathPattern, path));

            // No match/not verifying
            if (endpoint == null)
            {
                return CreateProceedResponse();
            }

            List<IRateLimitRule>? endpointRules = null;

            try
            {
                endpointRules = GetEndpointRules(endpoint);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, null);
                throw;
            }

            // All rules should share the same timestamp rather than creating their own
            var timestamp = DateTime.UtcNow;

            // TODO: Consider running rules in parallel as an opitmization. Could queue up rules in a concurrent queue,
            // start n TPL tasks that pluck items from the queue and signal cancellation when any
            // rule fails. Leverage Task.WaitAll and check task results for any failure
            foreach (var rule in endpointRules)
            {
                var result = await rule.Verify(context, user, endpoint, timestamp);
                if (!result.Proceed)
                {
                    return result;
                }
            }

            return CreateProceedResponse();
        }

        public void AddRulesFactory(IRateLimitRuleFactory rulesFactory)
        {
            if (rulesFactory == null)
            {
                throw new ArgumentNullException(nameof(rulesFactory));
            }

            _rateLimitRuleFactories.Add(rulesFactory);
        }

        private bool IsMatch(string pathPattern, PathString path)
        {
            var regex = new Regex(pathPattern);

            return regex.IsMatch(path.Value);
        }

        private List<IRateLimitRule> GetEndpointRules(IEndpoint endpoint)
        {
            var rulesKey = (endpoint.PathPattern, endpoint.Verbs);

            if (_rulesLookup.ContainsKey(rulesKey))
            {
                return _rulesLookup[rulesKey];
            }

            var rules = new List<IRateLimitRule>();

            // TODO: Consider building rules ahead of time (rather than once/lazily) as a potential optimization
            foreach (var configuration in endpoint.Rules)
            {
                var ruleFactory = _rateLimitRuleFactories.FirstOrDefault(rf => rf.SupportsType(configuration.Type)) ??
                    throw new InvalidOperationException($"Could not find a rule factory for creating {configuration.Type} rules.");

                var rule = ruleFactory.Create(configuration);
                rules.Add(rule);
            }

            _rulesLookup[rulesKey] = rules;

            return rules;
        }

        private IRateLimitRuleResult CreateProceedResponse()
        {
            return new RateLimitRuleResult(true, null);
        }
    }
}
