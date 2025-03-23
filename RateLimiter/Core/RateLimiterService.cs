using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RateLimiter.ConfigurationStorageProvider;
using RateLimiter.Rules.Interfaces;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Core
{
    public class RateLimiterService(IConfigurationStorageProvider<IRateLimiterRule> configurationStorageProvider, ILogger<RateLimiterService> logger)
    {
        private readonly IConfigurationStorageProvider<IRateLimiterRule> _configurationStorageProvider = configurationStorageProvider;
        private readonly ILogger<RateLimiterService> _logger = logger;

        public async Task<bool> InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var endpoint = context.Request.Path.ToString();

            if ((string.IsNullOrEmpty(token)) || (String.IsNullOrEmpty(endpoint)))
                return false;

            var requestContext = new ClientRequestContext(token, endpoint);

            var configProvider = new ConfigurationProvider(_configurationStorageProvider);

            var rules = await configProvider.GetConfigAsync(endpoint);

            if (rules == null || rules.Count==0)
            {
                _logger.LogWarning("No rules defined for this Endpoint");

                return true; // No rules defined for this endpoint
            }

            foreach (var rule in rules)
            {
                if (!await rule.IsRequestAllowedAsync(requestContext))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
